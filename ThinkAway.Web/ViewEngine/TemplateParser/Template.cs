#region License
//=============================================================================
// ThinkAway MVC - .NET Web Application Framework 
//
// Copyright (c) 2003-2009 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Caching;
using ThinkAway.Core.Parser;

namespace ThinkAway.Web
{
    internal class Template
    {
        private readonly CompiledTemplate _parsedTemplate;
        private string _headSection;
        private readonly string _pageTitle;
        private readonly string _destinationPath;
        private readonly string _fileName;

        private static readonly object _templateCacheLock = new object();
        private static readonly TemplateRenderer _templateParser = new TemplateRenderer();

        public static Template CreateTemplate(string templateFile, string destinationPath, bool onlyBody)
        {
            if (WebAppContext.Offline)
                return new Template(templateFile,destinationPath, onlyBody);

            string key = "TPL: " + destinationPath + ";" + templateFile;

            Template template = (Template) WebAppContext.WebCache[key];

            if (template != null)
                return template;

            lock (_templateCacheLock)
            {
                template = (Template) WebAppContext.WebCache[key];

                if (template != null)
                    return template;

                template = new Template(templateFile, destinationPath, onlyBody);

                WebAppContext.WebCache.Insert(key, template, new CacheDependency(templateFile), Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
            }

            return template;
        }

        public static Template CreateTemplate(Template callingTemplate, string templateName, bool onlyBody)
        {
            return CreateTemplate(templateName, callingTemplate._destinationPath, onlyBody);
        }

        private Template(string templateFile, string destinationPath, bool onlyBody)
        {
            _destinationPath = destinationPath;
            _fileName = templateFile;

            string contents = TemplateUtil.ReadTemplateContents(_fileName, _destinationPath);

            Match matchTitle = Regex.Match(contents, @"<title\s*>(?<title>.*?)</title>");
            Match matchCopyToMaster = Regex.Match(contents, @"<!--\s*#STARTCOPY#\s*-->(?<text>.*?)<!--\s*#ENDCOPY#\s*-->", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (matchTitle.Success)
                _pageTitle = matchTitle.Groups["title"].Value;

            if (matchCopyToMaster.Success)
                _headSection = matchCopyToMaster.Groups["text"].Value;

            if (onlyBody)
                contents = TemplateUtil.ExtractBody(contents);

            _parsedTemplate = PreParseTemplate(contents);
            _parsedTemplate.FileName = templateFile;
        }

        public string PageTitle
        {
            get { return _pageTitle; }
        }

        public string HeadSection
        {
            get { return _headSection; }
        }

        public string Render(View view)
        {
            try
            {
                return _templateParser.Render(_parsedTemplate, new TemplateParserContext(view, this));
            }
            catch (TemplateParserException ex)
            {
                throw new ThinkAwayMvcException("Error rendering template " + Path.GetFileName(_fileName), ex);
            }
        }

        public string GetSubTemplatePath(string templateName)
        {
            if (templateName.StartsWith("~/") || templateName.StartsWith("/"))
                return WebAppContext.Server.MapPath(templateName);
            else
                return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(_fileName), templateName));
        }

        public string ReadSubTemplate(string templateName)
        {
            string templatePath = GetSubTemplatePath(templateName);

            return TemplateUtil.ExtractBody(TemplateUtil.ReadTemplateContents(templatePath, _destinationPath));
        }

        public CompiledTemplate CompileSubTemplate(string templateName)
        {
            string templatePath = GetSubTemplatePath(templateName);

            Template innerTemplate = CreateTemplate(this, templatePath, true);

            _headSection += innerTemplate._headSection;

            return innerTemplate._parsedTemplate;
        }

        private CompiledTemplate PreParseTemplate(string unparsedText)
        {
            try
            {
                return _templateParser.Parse(unparsedText);
            }
            catch (TemplateParserException ex)
            {
                throw new ThinkAwayMvcException("Error parsing template " + Path.GetFileName(_fileName), ex);
            }
        }

    }
}

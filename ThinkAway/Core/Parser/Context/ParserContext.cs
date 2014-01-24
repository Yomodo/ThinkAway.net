using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using ThinkAway.Core.Parser.Context;

namespace ThinkAway.Core.Parser
{
    public class ParserContext : IParserContext
    {

        public delegate bool VariableResolver(string varName, out object var, out Type varType);

 

        // Fields
        private AssignmentPermissions _assignmentPermissions;
        private bool _caseSensitiveVariables;
        private bool _emptyCollectionIsFalse;
        private bool _emptyStringIsFalse;
        private IFormatProvider _formatProvider;
        private bool _nonEmptyStringIsTrue;
        private bool _notNullIsTrue;
        private bool _notZeroIsTrue;
        private bool _nullIsFalse;
        private readonly IParserContext _parentContext;
        private bool _returnNullWhenNullReference;
        private readonly object _rootObject;
        private StringComparison _stringComparison;
        private Dictionary<string, Type> _types;
        private Dictionary<string, object> _variables;
        public VariableResolver MissingVariableHandler;

        // Methods
        public ParserContext()
        {
            this._variables = new Dictionary<string, object>();
            this._types = new Dictionary<string, Type>();
            this._caseSensitiveVariables = true;
            this._stringComparison = StringComparison.InvariantCulture;
            this._formatProvider = NumberFormatInfo.InvariantInfo;
        }

        public ParserContext(object rootObject)
        {
            this._variables = new Dictionary<string, object>();
            this._types = new Dictionary<string, Type>();
            this._caseSensitiveVariables = true;
            this._stringComparison = StringComparison.InvariantCulture;
            this._formatProvider = NumberFormatInfo.InvariantInfo;
            this._rootObject = rootObject;
        }

        protected ParserContext(ParserContext parentContext)
        {
            this._variables = new Dictionary<string, object>();
            this._types = new Dictionary<string, Type>();
            this._caseSensitiveVariables = true;
            this._stringComparison = StringComparison.InvariantCulture;
            this._formatProvider = NumberFormatInfo.InvariantInfo;
            if (parentContext != null)
            {
                this._parentContext = parentContext;
                this._assignmentPermissions = parentContext._assignmentPermissions;
                this._nullIsFalse = parentContext._nullIsFalse;
                this._notNullIsTrue = parentContext._notNullIsTrue;
                this._notZeroIsTrue = parentContext._notZeroIsTrue;
                this._emptyStringIsFalse = parentContext._emptyStringIsFalse;
                this._nonEmptyStringIsTrue = parentContext._nonEmptyStringIsTrue;
                this._returnNullWhenNullReference = parentContext._returnNullWhenNullReference;
                this._emptyCollectionIsFalse = parentContext._emptyCollectionIsFalse;
                this._caseSensitiveVariables = parentContext._caseSensitiveVariables;
                this._stringComparison = parentContext._stringComparison;
                this._formatProvider = parentContext._formatProvider;
            }
        }

        public void AddFunction(string name, MethodInfo methodInfo)
        {
            this.Set<object>(name, ContextFactory.CreateFunction(methodInfo));
        }

        public void AddFunction(string name, MethodInfo methodInfo, object targetObject)
        {
            this.Set<object>(name, ContextFactory.CreateFunction(methodInfo, targetObject));
        }

        public void AddFunction(string name, Type type, string methodName)
        {
            this.Set<object>(name, ContextFactory.CreateFunction(type, methodName));
        }

        public void AddFunction(string name, Type type, string methodName, object targetObject)
        {
            this.Set<object>(name, ContextFactory.CreateFunction(type, methodName, targetObject));
        }

        public void AddType(string name, Type type)
        {
            this.Set<object>(name, ContextFactory.CreateType(type));
        }

        public virtual IParserContext CreateLocal()
        {
            return new ParserContext(this);
        }

        public virtual bool Exists(string varName)
        {
            return (this._variables.ContainsKey(varName) || (((this._rootObject != null) && PropertyHelper.Exists(this._rootObject, varName)) || ((this._parentContext != null) && this._parentContext.Exists(varName))));
        }

        public string Format(string formatString, params object[] parameters)
        {
            return string.Format(this.FormatProvider, formatString, parameters);
        }

        public virtual bool Get(string varName, out object value, out Type type)
        {
            if (this._variables.ContainsKey(varName))
            {
                value = this._variables[varName];
                type = this._types[varName];
            }
            else
            {
                if ((this._rootObject != null) && PropertyHelper.TryGetValue(this._rootObject, varName, out value, out type))
                {
                    return true;
                }
                if ((this._parentContext == null) || !this._parentContext.Get(varName, out value, out type))
                {
                    value = null;
                    type = typeof(object);
                    return false;
                }
            }
            if ((type == typeof(object)) && (value != null))
            {
                type = value.GetType();
            }
            return true;
        }

        public void Set<T>(string name, T data)
        {
            this.Set(name, data, typeof(T));
        }

        public void Set(string name, IValueWithType data)
        {
            this.Set(name, data.Value, data.Type);
        }

        public void Set(string name, object data, Type type)
        {
            if ((this._parentContext != null) && this._parentContext.Exists(name))
            {
                this._parentContext.Set(name, data, type);
            }
            this.SetLocal(name, data, type);
        }

        public void SetLocal<T>(string name, T data)
        {
            this.SetLocal(name, data, typeof(T));
        }

        public void SetLocal(string name, IValueWithType data)
        {
            this.SetLocal(name, data.Value, data.Type);
        }

        public void SetLocal(string name, object data, Type type)
        {
            this._variables[name] = data;
            this._types[name] = type;
        }

        public bool ToBoolean(object value)
        {
            if (value is bool)
            {
                return (bool) value;
            }
            if (this._notZeroIsTrue)
            {
                if ((((value is int) || (value is uint)) || ((value is short) || (value is ushort))) || (((value is long) || (value is ulong)) || ((value is byte) || (value is sbyte))))
                {
                    return (Convert.ToInt64(value) != 0L);
                }
                if (value is decimal)
                {
                    return (((decimal) value) != 0M);
                }
                if ((value is float) || (value is double))
                {
                    return (Convert.ToDouble(value) == 0.0);
                }
            }
            if ((value is ICollection) && this._emptyCollectionIsFalse)
            {
                return (((ICollection) value).Count > 0);
            }
            if ((value is IEnumerable) && this._emptyCollectionIsFalse)
            {
                return ((IEnumerable) value).GetEnumerator().MoveNext();
            }
            if ((this._nonEmptyStringIsTrue && (value is string)) && (((string) value).Length > 0))
            {
                return true;
            }
            if ((this._emptyStringIsFalse && (value is string)) && (((string) value).Length == 0))
            {
                return false;
            }
            if (this._notNullIsTrue && (value != null))
            {
                return true;
            }
            if (this._nullIsFalse && (value == null))
            {
                return false;
            }
            if (this._parentContext == null)
            {
                throw new NullReferenceException();
            }
            return this._parentContext.ToBoolean(value);
        }

        // Properties
        public AssignmentPermissions AssignmentPermissions
        {
            get
            {
                return this._assignmentPermissions;
            }
            set
            {
                this._assignmentPermissions = value;
            }
        }

        public bool CaseSensitiveVariables
        {
            get
            {
                return this._caseSensitiveVariables;
            }
            set
            {
                if (value != this._caseSensitiveVariables)
                {
                    if (value)
                    {
                        this._variables = new Dictionary<string, object>();
                        this._types = new Dictionary<string, Type>();
                    }
                    else
                    {
                        this._variables = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                        this._types = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
                    }
                    this._caseSensitiveVariables = value;
                }
            }
        }

        public bool EmptyCollectionIsFalse
        {
            get
            {
                return this._emptyCollectionIsFalse;
            }
            set
            {
                this._emptyCollectionIsFalse = value;
            }
        }

        public bool EmptyStringIsFalse
        {
            get
            {
                return this._emptyStringIsFalse;
            }
            set
            {
                this._emptyStringIsFalse = value;
            }
        }

        public IFormatProvider FormatProvider
        {
            get
            {
                return this._formatProvider;
            }
            set
            {
                this._formatProvider = value;
            }
        }

        public bool NonEmptyStringIsTrue
        {
            get
            {
                return this._nonEmptyStringIsTrue;
            }
            set
            {
                this._nonEmptyStringIsTrue = value;
            }
        }

        public bool NotNullIsTrue
        {
            get
            {
                return this._notNullIsTrue;
            }
            set
            {
                this._notNullIsTrue = value;
            }
        }

        public bool NotZeroIsTrue
        {
            get
            {
                return this._notZeroIsTrue;
            }
            set
            {
                this._notZeroIsTrue = value;
            }
        }

        public bool NullIsFalse
        {
            get
            {
                return this._nullIsFalse;
            }
            set
            {
                this._nullIsFalse = value;
            }
        }

        public bool ReturnNullWhenNullReference
        {
            get
            {
                return this._returnNullWhenNullReference;
            }
            set
            {
                this._returnNullWhenNullReference = value;
            }
        }

        public StringComparison StringComparison
        {
            get
            {
                return this._stringComparison;
            }
            set
            {
                this._stringComparison = value;
            }
        }
    }
}

 
 

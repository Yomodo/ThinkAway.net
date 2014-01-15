namespace ThinkAway.Core.Parser.Parsers.CSharp
{
    public class CSharpContext : ParserContext
    {
        // Methods
        public CSharpContext()
        {
            base.AddType("int", typeof(int));
            base.AddType("uint", typeof(uint));
            base.AddType("long", typeof(long));
            base.AddType("ulong", typeof(ulong));
            base.AddType("short", typeof(short));
            base.AddType("ushort", typeof(ushort));
            base.AddType("double", typeof(double));
            base.AddType("float", typeof(float));
            base.AddType("bool", typeof(bool));
            base.AddType("char", typeof(char));
            base.AddType("byte", typeof(byte));
            base.AddType("sbyte", typeof(sbyte));
            base.AddType("string", typeof(string));
            base.Set("null", null, typeof(object));
            base.Set<bool>("true", true);
            base.Set<bool>("false", false);
        }

        public CSharpContext(object rootObject) : base(rootObject)
        {
        }

        public CSharpContext(CSharpContext parentContext) : base((ParserContext) parentContext)
        {
        }

        public override IParserContext CreateLocal()
        {
            return new CSharpContext(this);
        }
    }
}

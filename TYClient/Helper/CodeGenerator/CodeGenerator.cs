namespace TY.SPIMS.Client.Helper.CodeGenerator
{
    public class CodeGenerator
    {
        public IGenerator Generator { get; set; }

        public CodeGenerator(IGenerator generator)
        {
            this.Generator = generator;
        }

        public string GenerateCode()
        {
            return Generator.GenerateCode();
        }
    }
}

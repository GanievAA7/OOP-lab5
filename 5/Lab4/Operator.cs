namespace Lab4
{
    public class Operator : OperatorMethod
    {
        public char symbolOperator;
        public EmptyOperatorMethod operatorMethod = null;
        public BinaryOperatorMethod binaryOperator = null;
        public TrinaryOperatorMethod trinaryOperator = null;
        public Operator(char symbolOperator)
        {
            this.symbolOperator = symbolOperator;
        }
    }
}

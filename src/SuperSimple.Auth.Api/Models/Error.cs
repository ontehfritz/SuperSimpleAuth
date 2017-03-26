namespace SuperSimple.Auth.Api
{
    //https://jsonapi.org/examples/#error-objects-multiple-errors
    public class Error
    {
        public int Status       { get; set; }
        public Source Source    { get; set; }
        public string Title     { get; set; }
        public string Detail    { get; set; }
    }

    public class Source
    {
        public string Pointer   { get; set; }
        public Source(string pointer)
        {
            Pointer = pointer;
        }
    }
}


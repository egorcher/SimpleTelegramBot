using System;

namespace TelegrammBott
{
    public class MessageInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string[] Text { get; set; }
        public string Path { get; set; }
        public DateTime DateTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace FileDBReader
{
    class CorrectEmptyClosingTag
    {
        private string CurrentTag = "";
        private bool InOpenTag = false;
        private Stack<string> TagHistory = new Stack<string>();
        private string LastChars = "";

        public void Advance(char CurrentChar)
        {
            LastChars = ((LastChars.Length >= 3) ? LastChars[1..] : LastChars) + CurrentChar;

            if (CurrentChar == '<')
            {
                InOpenTag = true;
            }
            else if (CurrentChar == '>' && InOpenTag)
            {
                InOpenTag = false;
                TagHistory.Push(CurrentTag);
                CurrentTag = "";
            }
            else if (CurrentChar == '/')
            {
                // cancel open tag
                InOpenTag = false;
            }
            else if (InOpenTag)
            {
                CurrentTag += CurrentChar;
            }
        }

        public bool IsClosing()
        {
            return LastChars.StartsWith("</");
        }

        public char[] GetCorrection()
        {
            // empty closing tag, return replacement
            if (LastChars.Last() == '>')
            {
                return TagHistory.Pop().ToCharArray();
            }
            // if non-empty, we still need to pop the stack
            else
            {
                TagHistory.Pop();
                return Array.Empty<char>();
            }
        }
    }
}

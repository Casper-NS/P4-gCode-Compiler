using GGCodeParser.analysis;
using GGCodeParser.node;
using System;
using System.Collections;

namespace PrettyPrintATestFile
{
    internal class TextPrinter : ReversedDepthFirstAdapter
    {
        private enum codes
        {
            ESC = 27
        };

        private enum style
        {
            NORMAL = 0,
            BOLD = 1,
            UNDERSCORE = 4,
            BLINK = 5,
            CONCEALED = 8
        };

        private enum fg_color
        {
            FG_BLACK = 30,
            FG_RED = 31,
            FG_GREEN = 32,
            FG_YELLOW = 33,
            FG_BLUE = 34,
            FG_MAGENTA = 35,
            FG_CYAN = 36,
            FG_WHITE = 37
        };

        private enum bg_color
        {
            BG_BLACK = 40,
            BG_RED = 41,
            BG_GREEN = 42,
            BG_YELLOW = 43,
            BG_BLUE = 44,
            BG_MAGENTA = 45,
            BG_CYAN = 46,
            BG_WHITE = 47
        };

        public TextPrinter()
        {
        }

        public override void OutStart(Start node)
        {
            Console.Write(TreeColor() + "\n" + output.Substring(3) + "\n" + ResetColor());
        }

        public override void DefaultIn(Node node)
        {
            if (last)
                indentchar.Push('`');
            else
                indentchar.Push('|');

            indent = indent + "   ";
            last = true;
        }

        public override void DefaultOut(Node node)
        {
            indent = indent.Substring(0, indent.Length - 3);
            indent = indent.Substring(0, indent.Length - 1) + indentchar.Peek();
            indentchar.Pop();
            output = indent + "- " + SetColor(style.NORMAL, fg_color.FG_GREEN,
                bg_color.BG_BLACK) + node.GetType().Name + TreeColor() + "\n" + output;
            indent = indent.Substring(0, indent.Length - 1) + "|";
        }

        public override void DefaultCase(Node node)
        {
            if (last) indent = indent.Substring(0, indent.Length - 1) + "`";
            string nodeText = ((Token)node).Text;
            if (((Token)node).Text == "\n")
            {
                nodeText = "EOL";
            }
            output = indent + "- " + SetColor(style.NORMAL, fg_color.FG_RED, bg_color.BG_BLACK) +
                nodeText + TreeColor() + "\n" + output;

            indent = indent.Substring(0, indent.Length - 1) + "|";

            last = false;
        }

        private string SetColor(style style, fg_color fgColor, bg_color bgColor)
        {
            if (color)
                return (char)codes.ESC + "[" + (int)style + ";" + (int)fgColor + "m";

            return "";
        }

        public override void CaseEOF(EOF node)
        {
            last = false;
        }

        private string ResetColor()
        {
            if (color)
                return (char)codes.ESC + "[0m";
            else
                return "";
        }

        private string TreeColor()
        {
            return SetColor(style.NORMAL, fg_color.FG_BLACK, bg_color.BG_BLACK);
        }

        public void SetColor(bool b)
        {
            color = b;
        }

        private string indent, output;
        private Stack indentchar = new Stack();
        private bool last = false;
        private bool color = false;
    }
}
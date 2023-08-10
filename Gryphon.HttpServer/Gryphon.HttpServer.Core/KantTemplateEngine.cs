using System.Text;

namespace Gryphon.HttpServer.Core
{
    public class KantTemplateEngine
    {
        public string InnerStart { get; set; }

        public string InnerEnd { get; set; }

        public string OuterStart { get; set; }

        public string OuterEnd { get; set; }

        public string Process(string template)
        {
            output = new StringBuilder();
            state = State.Symbol;

            output.Append(OuterStart);

            foreach (char ch in template)
            {
                Input symbol = ch switch
                {
                    '<' => Input.LeftBracket,
                    '@' => Input.At,
                    '=' => Input.Equal,
                    '>' => Input.RightBracket,
                    _ => Input._
                };

                switch (state, symbol)
                {
                    // Symbol
                    case (State.Symbol, Input.LeftBracket):
                        state = State.LeftBracket; 
                        break;
                    case (State.Symbol, Input._):
                        if (ch == '"')
                        {
                            output.Append("\\");
                        }
                        output.Append(ch);
                        break;
                    // LeftBracket
                    case (State.LeftBracket, Input.At):
                        state = State.At;
                        output.Append(OuterEnd);
                        break;
                    case (State.LeftBracket, _):
                        state = State.Symbol;
                        output.Append("<");
                        output.Append(ch);
                        break;
                    // At
                    case (State.At or State.AtDot, Input.At):
                        state = State.EndAt;                        
                        break;
                    case (State.At, Input.Equal):
                        state = State.Equal;
                        output.Append(InnerStart);
                        break;
                    case (State.At, _):
                        state = State.AtDot;
                        output.Append(ch);
                        break;
                    // Equal
                    case (State.Equal, Input.At):
                        state = State.EndEqual;
                        break;
                    // EndEqual, EndAt
                    case (State.EndEqual, Input.RightBracket):
                        output.Append(InnerEnd);
                        goto EndAt;
                    case (State.EndAt, Input.RightBracket):
                    EndAt:
                        state = State.Symbol;
                        output.Append(OuterStart);
                        break;
                    case (State.EndEqual or State.EndAt, _):
                        output.Append("@");
                        output.Append(ch);
                        break;
                    // Rest
                    case (_, _):
                        output.Append(ch);
                        break;

                }
            }

            output.Append(OuterEnd);

            return output.ToString();
        }

        private State state;
        private StringBuilder output;

        private enum State
        {
            Symbol, LeftBracket, At, Equal, EndEqual, AtDot, EndAt
        }

        private enum Input
        {
            LeftBracket, At, Equal, RightBracket, _
        }
    }
}
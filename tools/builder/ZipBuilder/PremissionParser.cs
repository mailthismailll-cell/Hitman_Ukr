using System.Text;

namespace ZipBuilder
{
    public static class PremissionParser
    {
        public static PremissionHLocClass Parse(string input)
        {
            var root = new PremissionHLocClass { FullKey = "" };
            var stack = new Stack<PremissionHLocClass>();
            stack.Push(root);

            var lines = input.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string? line = lines[i];
                var trimmedLine = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                if (trimmedLine.EndsWith("["))
                {
                    var key = trimmedLine.TrimEnd(' ', '[').Trim('"');
                    var parent = stack.Peek();
                    var newNode = new PremissionHLocClass
                    {
                        Key = key,
                        FullKey = parent.FullKey == "" ? key : $"{parent.FullKey}.{key}"
                    };
                    parent.Children.Add(newNode);
                    stack.Push(newNode);
                }
                else if (trimmedLine == "]")
                {
                    stack.Pop();
                }
                else
                {
                    int? currentNum1 = null; int? currentNum2 = null;
                    if (trimmedLine.Contains(" = \""))
                    {
                        var key = trimmedLine.Split(" = \"")[0].Replace("\"", "");

                        var value = trimmedLine.Split(" = \"")[1];
                        if (IsEndString(value) || IsEndWithDigits(value))
                        {
                            if (IsEndWithDigits(value))
                            {
                                var digits = value.Split(" ");
                                if (digits.Count() > 2 && digits[digits.Length - 3].EndsWith("\""))
                                {
                                    currentNum1 = Convert.ToInt32(digits[digits.Length - 2]);
                                    currentNum2 = Convert.ToInt32(digits[digits.Length - 1]);
                                    var val = string.Join(" ", digits.Take(digits.Length - 2));
                                    value = val.Substring(0, val.Length - 1);
                                }
                                if (digits.Count() > 1 && digits[digits.Length - 2].EndsWith("\""))
                                {
                                    currentNum1 = Convert.ToInt32(digits[digits.Length - 1]);
                                    var val = string.Join(" ", digits.Take(digits.Length - 1));
                                    value = val.Substring(0, val.Length - 1); 
                                }
                            }
                            if (IsEndString(value))
                            {
                                value = value.Substring(0, value.Length - 1);
                            }

                        }
                        else
                        {
                            for (; ; )
                            {
                                i++;
                                string? innerline = lines[i];
                                var trimmedInnerline = innerline.Trim();
                                if (IsEndString(trimmedInnerline) || IsEndWithDigits(trimmedInnerline))
                                {
                                    if (IsEndWithDigits(trimmedInnerline))
                                    {
                                        var digits = trimmedInnerline.Split(" ");
                                        if (digits.Count() > 2 && digits[digits.Length - 3].EndsWith("\""))
                                        {
                                            currentNum1 = Convert.ToInt32(digits[digits.Length - 2]);
                                            currentNum2 = Convert.ToInt32(digits[digits.Length - 1]);
                                            var val = string.Join(" ", digits.Take(digits.Length - 2));
                                            trimmedInnerline = val.Substring(0, val.Length - 1);
                                        }
                                        if (digits.Count() > 1 && digits[digits.Length - 2].EndsWith("\""))
                                        {
                                            currentNum1 = Convert.ToInt32(digits[digits.Length - 1]);
                                            var val = string.Join(" ", digits.Take(digits.Length - 1));
                                            trimmedInnerline = val.Substring(0, val.Length - 1);
                                        }
                                    }
                                    if (IsEndString(trimmedInnerline))
                                    {
                                        trimmedInnerline = trimmedInnerline.Substring(0, trimmedInnerline.Length - 1);
                                    }
                                    value += $"\n{trimmedInnerline}";
                                    break;
                                }
                                else
                                {
                                    value += $"\n{trimmedInnerline}";
                                }
                            }
                        }

                        var parent = stack.Peek();
                        parent.Children.Add(new PremissionHLocClass
                        {
                            Key = key,
                            Value = value,
                            Num1 = currentNum1,
                            Num2 = currentNum2,
                            FullKey = $"{parent.FullKey}.{key}"
                        });
                    }
                }

            }



            return root;
        }

        private static bool IsEndString(string str)
        {
            return str.EndsWith("\"");
        }

        private static bool IsEndWithDigits(string str)
        {
            return str.EndsWith("0") || str.EndsWith("1") || str.EndsWith("2") || str.EndsWith("3") || str.EndsWith("4") || str.EndsWith("5")
                 || str.EndsWith("6") || str.EndsWith("7") || str.EndsWith("8") || str.EndsWith("9");
        }


        public static string Serialize(PremissionHLocClass node, int indentLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            SerializeNode(node, sb, indentLevel);
            return sb.ToString();
        }

        private static void SerializeNode(PremissionHLocClass node, StringBuilder sb, int indentLevel)
        {
            string indent = new string('\t', indentLevel);

            if (!string.IsNullOrEmpty(node.Value)) // ÷е ключ-значенн€
            {
                sb.Append(indent);
                sb.Append($"\"{node.Key}\" = \"{Escape(node.Value)}\"");

                if (node.Num1.HasValue && node.Num2.HasValue)
                {
                    sb.Append($" {node.Num1.Value} {node.Num2.Value}");
                }

                sb.AppendLine();
            }
            else if (node.Children.Count > 0) // ÷е об'Їкт (м≥стить доч≥рн≥ елементи)
            {
                sb.Append(indent);
                sb.AppendLine($"\"{node.Key}\" [");

                foreach (var child in node.Children)
                {
                    SerializeNode(child, sb, indentLevel + 1);
                }

                sb.Append(indent);
                sb.AppendLine("]");
            }
        }

        private static string Escape(string value)
        {
            return value.Replace("\"", "\\\""); // ≈крануЇмо лапки всередин≥ значень
        }
    }
}
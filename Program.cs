using System;
using System.Collections.Generic;
using System.Text;

public class ManualLexer 
{
    public enum TokenType
    {
        Integer,
        Float,
        Identifier,
        Operator,
        Parenthesis,
        EOF,
        Invalid
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }

    public static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>(); 
        StringBuilder currentToken = new StringBuilder(); 
        int index = 0; 

        while (index < input.Length)
        {
            char currentChar = input[index];

            if (char.IsWhiteSpace(currentChar))
            {
                index++;
                continue; 
            }

            if (char.IsDigit(currentChar))
            {
                currentToken.Clear();

                while (index < input.Length && char.IsDigit(input[index]))
                {
                    currentToken.Append(input[index]);
                    index++;
                }
                if (index < input.Length && input[index] == '.')
                {
                    currentToken.Append(input[index]);
                    index++;

                    while (index < input.Length && char.IsDigit(input[index]))
                    {
                        currentToken.Append(input[index]);
                        index++;
                    }
                    tokens.Add(new Token(TokenType.Float, currentToken.ToString()));
                }
                else
                {
                   tokens.Add(new Token(TokenType.Integer, currentToken.ToString()));
                }
                continue;
            }
            if (char.IsLetter(currentChar) || currentChar == '_')
            {
                currentToken.Clear(); 
                while (index < input.Length && (char.IsLetterOrDigit(input[index]) || input[index] == '_'))
                {
                    currentToken.Append(input[index]);
                    index++;
                }

                tokens.Add(new Token(TokenType.Identifier, currentToken.ToString()));
                continue;
            }
            if ("+-*/=<>!".Contains(currentChar))
            {
                currentToken.Clear();
                currentToken.Append(currentChar);
                index++;

                tokens.Add(new Token(TokenType.Operator, currentToken.ToString()));
                continue; 
            }
            if (currentChar == '(' || currentChar == ')')
            {
                currentToken.Clear();
                currentToken.Append(currentChar);
                index++;

                tokens.Add(new Token(TokenType.Parenthesis, currentToken.ToString()));
                continue;
            }

            tokens.Add(new Token(TokenType.Invalid, currentChar.ToString()));
            index++;
        }

        tokens.Add(new Token(TokenType.EOF, "EOF"));

        return tokens;
    }

    public static string PrintTokens(string input)
    {
        var tokens = Tokenize(input);
        StringBuilder output = new StringBuilder();
        foreach (var token in tokens)
        {
            output.AppendLine(token.ToString()); 
        }
        return output.ToString(); 
    }
}

public class SymbolTable
{
    private HashSet<string> _declaredVariables = new HashSet<string>();

    public void DeclareVariable(string name)
    {
        if (_declaredVariables.Contains(name))
        {
            throw new Exception($"Variable '{name}' already declared.");
        }
        _declaredVariables.Add(name);
    }

    public bool IsDeclared(string name)
    {
        return _declaredVariables.Contains(name);
    }
}

public abstract class AstNode
{
    public abstract void Print(int indentLevel = 0, StringBuilder output = null);
    public abstract double Evaluate();
}

public class NumberNode : AstNode
{
    public string Value { get; set; }

    public NumberNode(string value)
    {
        Value = value;
    }

    public override void Print(int indentLevel = 0, StringBuilder output = null)
    {
        output.AppendLine(new string(' ', indentLevel * 2) + "Number: " + Value);
    }

    public override double Evaluate()
    {
        return double.Parse(Value);
    }
}

public class IdentifierNode : AstNode
{
    public string Name { get; set; }

    public IdentifierNode(string name)
    {
        Name = name;
    }

    public override void Print(int indentLevel = 0, StringBuilder output = null)
    {
        output.AppendLine(new string(' ', indentLevel * 2) + "Identifier: " + Name);
    }

    public override double Evaluate()
    {
        throw new NotImplementedException("Identifier evaluation not implemented.");
    }
}

public class BinaryOperationNode : AstNode
{ 
    public string Operator { get; set; }
    public AstNode Left { get; set; }
    public AstNode Right { get; set; }

    public BinaryOperationNode(string operatorSymbol, AstNode left, AstNode right)
    {
        Operator = operatorSymbol; 
        Left = left; 
        Right = right; 
    }

    public override void Print(int indentLevel = 0, StringBuilder output = null)
    {
        output.AppendLine(new string(' ', indentLevel * 2) + "Operator: " + Operator);
        Left.Print(indentLevel + 1, output);
        Right.Print(indentLevel + 1, output);
    }

    public override double Evaluate()
    {
        double leftValue = Left.Evaluate();
        double rightValue = Right.Evaluate();

        switch (Operator)
        {
            case "+":
                return leftValue + rightValue;
            case "-":
                return leftValue - rightValue;
            case "*":
                return leftValue * rightValue;
            case "/":
                if (rightValue == 0)
                {
                    throw new Exception("Division by zero.");
                }
                return leftValue / rightValue;
            default:
                throw new Exception("Unknown operator: " + Operator);
        }
    }
}

public class ParenthesesNode : AstNode
{
    public AstNode Expression { get; set; }

    public ParenthesesNode(AstNode expression)
    {
        Expression = expression;
    }

    public override void Print(int indentLevel = 0, StringBuilder output = null)
    {
        output.AppendLine(new string(' ', indentLevel * 2) + "Parentheses:");
        Expression.Print(indentLevel + 1, output);
    }

    public override double Evaluate()
    {
        return Expression.Evaluate();
    }
}

public class ManualParserWithSemantics
{
    private List<ManualLexer.Token> _tokens; 
    private int _currentTokenIndex; 
    private SymbolTable _symbolTable; 
        
    public ManualParserWithSemantics(List<ManualLexer.Token> tokens)  
    {
        _tokens = tokens;
        _currentTokenIndex = 0;
        _symbolTable = new SymbolTable();
    }

    private ManualLexer.Token CurrentToken => _tokens[_currentTokenIndex];
    private ManualLexer.Token NextToken => (_currentTokenIndex + 1 < _tokens.Count) ? _tokens[_currentTokenIndex + 1] : new ManualLexer.Token(ManualLexer.TokenType.EOF, "EOF");

    public AstNode ParseExpression()
    {
        var left = ParseTerm();

        while (CurrentToken.Type == ManualLexer.TokenType.Operator &&  
               (CurrentToken.Value == "+" || CurrentToken.Value == "-"))
        {
            string op = CurrentToken.Value; 
            _currentTokenIndex++; 
            var right = ParseTerm(); 
            left = new BinaryOperationNode(op, left, right); 
        }

        return left; 
    }

    public AstNode ParseTerm() 
    {
        var left = ParseFactor(); 

        while (CurrentToken.Type == ManualLexer.TokenType.Operator && 
               (CurrentToken.Value == "*" || CurrentToken.Value == "/"))
        {
            string op = CurrentToken.Value;
            _currentTokenIndex++;
            var right = ParseFactor();
            left = new BinaryOperationNode(op, left, right);
        }

        return left; 
    }

    public AstNode ParseFactor() 
    {
         if (CurrentToken.Type == ManualLexer.TokenType.Integer || CurrentToken.Type == ManualLexer.TokenType.Float)
        {
            var numberNode = new NumberNode(CurrentToken.Value);
            _currentTokenIndex++; 
            return numberNode; 
        
        }
        else if (CurrentToken.Type == ManualLexer.TokenType.Identifier)
        {
            string identifier = CurrentToken.Value; 
            if (!_symbolTable.IsDeclared(identifier)) 
            {
                throw new Exception($"Variable '{identifier}' is undeclared.");
            }
            var identifierNode = new IdentifierNode(identifier); 
            _currentTokenIndex++;
            return identifierNode;
        }
        else if (CurrentToken.Type == ManualLexer.TokenType.Parenthesis && CurrentToken.Value == "(")
        {
            _currentTokenIndex++; 
            var expression = ParseExpression(); 
            if (CurrentToken.Type == ManualLexer.TokenType.Parenthesis && CurrentToken.Value == ")")
            {
                _currentTokenIndex++; 
                return new ParenthesesNode(expression); 
            }
            else
            {
                throw new Exception("Expected closing parenthesis");
            }
        }
        throw new Exception("Unexpected token: " + CurrentToken.Value);
    }

    public void DeclareVariable(string name)
    {
        _symbolTable.DeclareVariable(name); 
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Enter values:");
        string input = Console.ReadLine();
        var tokens = ManualLexer.Tokenize(input);

        StringBuilder output = new StringBuilder();

        output.AppendLine("Tokens:");
        output.AppendLine(ManualLexer.PrintTokens(input));

        var parser = new ManualParserWithSemantics(tokens); 
        try
        {
            var ast = parser.ParseExpression();
            output.AppendLine("\nAbstract Syntax Tree (AST):");
            ast.Print(0, output); 

            double result = ast.Evaluate();
            output.AppendLine("\nFinal Output: User, your final output is: " + result);
        }
        catch (Exception ex)
        {
            output.AppendLine("Error: " + ex.Message); 
        }

        Console.WriteLine(output.ToString());
    }
}

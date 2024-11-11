### Overview

--took me 4 days . Started Thursday , Finished Monday morning so thats nice.
--next is an interpreter they both have tokens and ast in common soooo  ??

1. **Tokenization, Parsing, and Evaluation**: 
   - **Tokenization** breaks the input into smaller units (tokens).
   - **Parsing** organizes these tokens into an Abstract Syntax Tree (AST).
   - **Evaluation** calculates the result from the AST.

2. **Lexer**: 
   - The lexer scans the input and converts it into tokens like numbers, operators, and parentheses.
   - It skips whitespace and identifies integers, floats, operators, and variables.

3. **Tokens**: 
   - Basic elements like numbers, operators (+, -, *, /), parentheses, and variable names.

4. **Parser**:
   - The parser arranges tokens into a tree (AST) following rules for the order of operations (precedence).
   - It processes expressions in a structured way, respecting parentheses and operator precedence.

5. **AST (Abstract Syntax Tree)**:
   - A tree-like structure where each node represents part of the expression (numbers, variables, operations).
   - Each node computes a value or represents an operation.

6. **Symbol Table**:
   - Tracks declared variables to avoid redeclaration and to make sure variables are used only after they are declared.

7. **Precedence**:
   - Defines the order of operations (e.g., parentheses > multiplication/division > addition/subtraction).

8. **Grammar Rules**:
   - Define how expressions are structured and evaluated (following BODMAS: Brackets, Orders, Division, Multiplication, Addition, Subtraction).

### Classes Explained

- **Lexer (ManualLexer Class)**:
   - Breaks input into tokens.
   - Identifies digits, operators, and parentheses.
   - Handles floats and variable names.
   - Marks unrecognized characters as errors and adds an EOF token at the end.

- **Symbol Table (SymbolTable Class)**:
   - Keeps track of declared variables using a hashset.
   - Throws an error if a variable is used before being declared.

- **AST (Abstract Syntax Tree)**:
   - Built by the parser from tokens.
   - Each node represents parts of the expression (numbers, operators, variables).
   - Nodes have `Print` and `Evaluate` methods for displaying and calculating values.

- **Parser (ManualParserWithSemantics Class)**:
   - Converts tokens into an AST using recursive descent parsing.
   - Handles operator precedence and parentheses.
   - Throws errors for undeclared variables or invalid syntax.

### Main Program Flow

1. **Input**: The user enters an expression.
2. **Tokenization**: The input is tokenized by the lexer.
3. **Parsing**: The lexer’s tokens are parsed into an AST.
4. **Display**: The program prints the structure of the AST.
5. **Evaluation**: The AST is evaluated to compute the result, which is printed.

### Error Handling

- **Undeclared Variables**: The parser throws an error if a variable is used before being declared.
- **Mismatched Parentheses**: The lexer or parser flags unmatched parentheses.
- **Invalid Tokens**: The lexer marks unrecognized characters as invalid tokens.

In summary:
- Tokenize input → Parse tokens into an AST → Evaluate the AST → Handle errors like undeclared variables or invalid syntax.

### References

- [Aho/Ullman/Sethi's "Compilers" book](https://stackoverflow.com/questions/5739133/are-there-any-fun-ways-to-learn-about-languages-grammars-parsing-and-compile) – How abstract syntax trees relate to parsing.
- [Write a compiler in 24 hours talk by Phil Trelford ](https://www.youtube.com/watch?v=5kHRmc-R9d8) – A practical overview of compiler basics.
- [Roslyn Blog](http://blogs.msdn.com/b/visualstudio/archive/2011/10/19/introducing-the-microsoft-roslyn-ctp.aspx) – Microsoft's compiler platform.
- [Meta compilers from Val Schorre's 1964 paper](http://www.ibm-1401.info/Meta-II-schorre.pdf) – A historical perspective on meta-compilers.
- [Lexical to an AST in C#](https://medium.com/@matthew47671280/lexical-to-an-ast-with-c-6e378f76ac7f) – A guide on creating an Abstract Syntax Tree in C#.
- [Implementing a Language in C#](https://www.alexwilson.io/implementing-language-csharp-3/) – A blog post on building a programming language with C#.
-[Talks about AST trees](https://www.youtube.com/watch?v=MnctEW1oL-E&t=248s&pp=ygUXam9uYXRoYW4gYmxvdyAtIHBhcnNpbmc%3D) - Jonathan Blow Parsers

---
## Questions I had, you might have too :)
### Why not just use `Split()` for tokenization?

- **No Context**: `split()` only divides by spaces, but tokens like `348+358-355` don’t have spaces. Tokenization handles this by recognizing patterns.
- **Decimal Points**: `split()` splits `3.14` into `3` and `.14`, but tokenization sees it as one number.
- **Comments**: `split()` doesn’t skip comments. A lexer ignores comments and focuses on valid code.

---

### What is Regex?

- **Regex** defines patterns for matching text (e.g., `\d+` for digits, `[+\-*/]` for operators).

---

### Lexeme vs Token?

- **Lexeme**: Actual text (e.g., `"Ellen"`).
- **Token**: The category of the lexeme (e.g., **Identifier** for `"Ellen"`).

---

### Why ignore Whitespace?

- Whitespace separates tokens but has no meaning, so it’s ignored to avoid extra complexity.

---

### Naming Tokens?

- **Language-Specific**: Keywords and punctuation have their own tokens.
- **Categories**: Group lexemes like identifiers, numbers, and strings. Ignore whitespace and comments.

---

### How do lexemes become tokens?

- **Patterns**: The lexer matches text to patterns (e.g., `[a-zA-Z_][a-zA-Z0-9_]*` for identifiers).
- **Greedy Matching**: The lexer picks the longest match (e.g., `"12"` as one number, not two digits).
- **Context**: The lexer uses context to decide meanings (e.g., `"int"` as a keyword, not an identifier).

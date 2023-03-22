using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibInterpreter.Lexer;
using Bau.Libraries.LibInterpreter.Lexer.Builder;
using Bau.Libraries.LibInterpreter.Models.Expressions;
using Bau.Libraries.LibInterpreter.Models.Symbols;
using Bau.Libraries.LibInterpreter.Models.Tokens;

namespace Bau.Libraries.LibJobProcessor.Manager.Interpreter;

/// <summary>
///		Conversor de expresiones desde cadena a <see cref="ExpressionsCollection"/>
/// </summary>
internal class ExpressionConversor
{
	// Constantes privadas
	private const string TokenVariable = nameof(TokenVariable);
	private const string TokenString = nameof(TokenString);
	private const string TokenNumber = nameof(TokenNumber);
	private const string TokenAnd = nameof(TokenAnd);
	private const string TokenOr = nameof(TokenOr);
	private const string TokenNot = nameof(TokenNot);
	private const string TokenDistinct = nameof(TokenDistinct);
	private const string TokenLogical = nameof(TokenLogical);
	private const string TokenRelational = nameof(TokenRelational);
	private const string TokenMath = nameof(TokenMath);
	// Variables privadas
	private LexerManager? _lexer;

	public ExpressionConversor(JobProjectInterpreter jobProjectInterpreter)
	{
		JobProjectInterpreter = jobProjectInterpreter;
	}

	/// <summary>
	///		Genera las reglas del analizador
	/// </summary>
	private LexerManager CreateLexer()
	{
		LexerManager lexer = new();
		RuleBuilder builder = new();

			// Genera las reglas
			builder.WithDelimited(TokenVariable, "{{", "}}")
					.WithDelimited(TokenString, "'", "'")
					.WithDefaultNumbers(TokenNumber)
					.WithDefaultLogicalOperators(TokenLogical)
					.WithDefaultRelationalOperators(TokenRelational)
					.WithDefaultMathOperators(TokenMath)
					.WithWords(TokenAnd, "and")
					.WithWords(TokenOr, "or")
					.WithWords(TokenNot, "not")
					.WithWords(TokenDistinct, "<>");
			// Añade las regalas al analizador léxico
			lexer.Rules.AddRange(builder.Build());
			// Devuelve el analizador
			return lexer;
	}

	/// <summary>
	///		Interpreta una expresión
	/// </summary>
	internal ExpressionsCollection ParseExpression(string expression)
	{
		ExpressionsCollection expresssions = new();

			// Añade las expresiones leídas de los token
			foreach (TokenBase token in Lexer.Parse(expression))
				expresssions.Add(CreateExpression(token));
			// Devuelve las expresiones
			return expresssions;
	}

	/// <summary>
	///		Crea una expresión a partir del token
	/// </summary>
	private ExpressionBase CreateExpression(TokenBase token)
	{
		return token.Type switch
			{
				TokenVariable => new ExpressionVariableIdentifier(token.Value),
				TokenString => new ExpressionConstant(SymbolModel.SymbolType.String, token.Value),
				TokenNumber => new ExpressionConstant(SymbolModel.SymbolType.Numeric, token.Value.GetDouble(0)),
				TokenAnd => new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.And),
				TokenOr => new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.Or),
				TokenNot => new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.Not),
				TokenDistinct => new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Distinct),
				TokenLogical => token.Value switch
									{
										">" => new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Greater),
										"<" => new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Less),
										">=" => new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.GreaterOrEqual),
										"<=" => new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.LessOrEqual),
										"==" => new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Equal),
										"!=" => new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Distinct),
										_ => new ExpressionError($"Uknown logical operator: {token.Value}")
									},
				TokenRelational => token.Value switch
									{
										"||" => new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.Or),
										"&&" => new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.And),
										"!" => new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.Not),
										_ => new ExpressionError($"Uknown relational operator: {token.Value}")
									},
				TokenMath => token.Value switch
									{ 
										"(" => new ExpressionParenthesis(true), 
										")" => new ExpressionParenthesis(false), 
										"+" => new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Sum), 
										"-" => new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Substract), 
										"*" => new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Multiply), 
										"/" => new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Divide), 
										"%" => new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Modulus), 
										_ => new ExpressionError($"Unknow math operator: {token.Value}")
									},
				_ => new ExpressionError($"Token unknown: {token.Value}")
			};
	}

	/// <summary>
	///		Intérprete
	/// </summary>
	internal JobProjectInterpreter JobProjectInterpreter { get; }

	/// <summary>
	///		Análizador léxico
	/// </summary>
	internal LexerManager Lexer 
	{
		get
		{
			// Crea el analizador léxico si no existía
			if (_lexer is null)
				_lexer = CreateLexer();
			// Devuelve el analizador léxico
			return _lexer;
		}
	}
}

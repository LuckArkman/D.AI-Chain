using FluentValidation;
using Aethos.Application.Transactions.Commands;

namespace Aethos.Application.Transactions.Validators;

/// <summary>
/// Sprint 40: Regras estritas de conformidade de transação L2.
/// </summary>
public class ProcessTransactionCommandValidator : AbstractValidator<ProcessTransactionCommand>
{
    public ProcessTransactionCommandValidator()
    {
        RuleFor(v => v.From)
            .NotEmpty().WithMessage("Endereço remetente (From) obrigatório.")
            .Must(addr => addr.StartsWith("0x")).WithMessage("Endereço deve ser em formato Hex (0x).")
            .Length(42).WithMessage("Endereço Ethereum inválido (Tamanho).");

        RuleFor(v => v.To)
            .NotEmpty().WithMessage("Endereço destino (To) obrigatório.")
            .Must(addr => addr.StartsWith("0x")).WithMessage("Endereço deve ser em formato Hex (0x).")
            .Length(42).WithMessage("Endereço Ethereum inválido (Tamanho).");

        RuleFor(v => v.Value)
            .GreaterThanOrEqualTo(0).WithMessage("Valor da transação não pode ser negativo.");
    }
}

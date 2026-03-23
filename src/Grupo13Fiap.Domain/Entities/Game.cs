using Grupo13Fiap.Domain.Enum;

namespace Grupo13Fiap.Domain.Entities;

public class Game : EntityBase
{
    public CategoryGameEnum Category { get; private set; }
    public string Nome { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public DateTime DisponibilizationDate { get; private set; }

    public Game(CategoryGameEnum category, string nome, string description, decimal price)
    {
        SetNome(nome);
        SetDescription(description);
        SetPrice(price);

        Category = category;
        DisponibilizationDate = DateTime.UtcNow;
    }

    // Métodos de domínio

    public void SetNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio");

        Nome = nome;
    }

    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Descrição não pode ser vazia");

        Description = description;
    }

    public void SetPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Preço não pode ser negativo");

        Price = price;
    }

    public void ChangeCategory(CategoryGameEnum category)
    {
        Category = category;
    }

    public void ScheduleDisponibilization(DateTime date)
    {
        if (date < DateTime.UtcNow)
            throw new ArgumentException("Data de disponibilização não pode estar no passado");

        DisponibilizationDate = date;
    }

    public bool IsAvailable()
    {
        return DateTime.UtcNow >= DisponibilizationDate;
    }
}
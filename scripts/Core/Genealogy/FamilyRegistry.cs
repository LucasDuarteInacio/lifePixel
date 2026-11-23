using LifeSimulator.Core.Characters;

namespace LifeSimulator.Core.Genealogy;

/// <summary>
/// Registro de todas as pessoas conhecidas da partida — família,
/// amigos, parceiros e ex. É o escopo da "simulação profunda" do
/// GDD §8.5: todo NPC registrado envelhece, adoece, se relaciona e
/// morre. Pessoas falecidas permanecem registradas para a árvore
/// genealógica e o histórico familiar (GDD §7.1–7.2).
/// </summary>
public sealed class FamilyRegistry
{
    private readonly Dictionary<Guid, Person> _people = new();

    public IReadOnlyCollection<Person> All => _people.Values;

    public Person? Find(Guid id) => _people.GetValueOrDefault(id);

    public bool Register(Person person) => _people.TryAdd(person.Id, person);

    /// <summary>
    /// Registra a pessoa e toda a sua rede conectada (pais, irmãos,
    /// filhos, cônjuge), transitivamente. Como só existe <see cref="Person"/>
    /// no grafo quem foi gerado, a caminhada é limitada pela própria
    /// geração sob demanda.
    /// </summary>
    public int RegisterTree(Person root)
    {
        var added = 0;
        var queue = new Queue<Person>();
        queue.Enqueue(root);
        while (queue.Count > 0)
        {
            var person = queue.Dequeue();
            if (!_people.TryAdd(person.Id, person))
                continue;
            added++;
            if (person.Mother is not null) queue.Enqueue(person.Mother);
            if (person.Father is not null) queue.Enqueue(person.Father);
            if (person.Partner is not null) queue.Enqueue(person.Partner);
            foreach (var sibling in person.Siblings) queue.Enqueue(sibling);
            foreach (var child in person.Children) queue.Enqueue(child);
        }
        return added;
    }
}

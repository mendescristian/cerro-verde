using System.Text;

namespace Testes.Common.Utils;

public static class FakeStrings
{
    public static string Criar(int tamanho, bool apenasNumeros = false)
    {
        var chars = apenasNumeros ? "0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var sb = new StringBuilder(tamanho);

        for (int i = 0; i < tamanho; i++)
        {
            sb.Append(chars[random.Next(chars.Length)]);
        }

        return sb.ToString();
    }
}

public static class FakeNumbers
{
    public static int CriarInt(int minimo = 0, int maximo = 1000)
    {
        var random = new Random();

        return random.Next(minimo, maximo);
    }

    public static long CriarLong(long minimo = 0, long maximo = 1000)
    {
        var random = new Random();

        return random.NextInt64(minimo, maximo);
    }

    public static decimal CriarDecimal(int minimo = 0, int maximo = 1000)
    {
        var random = Random.Shared;

        return random.NextInt64(minimo, maximo);
    }
}

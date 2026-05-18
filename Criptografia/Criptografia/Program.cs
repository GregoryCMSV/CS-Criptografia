using System.Numerics;
using System.Text;

namespace Criptografia
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Separador("FUNDAMENTOS DE CRIPTOGRAFIA — Implementação C#");

            DemoCesar();
            DemoVigenere();
            DemoXor();
            DemoHash();
            DemoRsa();

            Separador("Fim das demonstrações. Pressione ENTER para sair.");
            Console.ReadLine();
        }

        static void Separador(string titulo)
        {
            Console.WriteLine("\n============================================================");
            Console.WriteLine($"  {titulo}");
            Console.WriteLine("============================================================");
        }

        // ====================================================================
        // SEÇÃO 1 — CIFRA DE CÉSAR
        // ====================================================================
        static string CesarCifrar(string texto, int deslocamento)
        {
            var resultado = new StringBuilder();
            foreach (char c in texto)
            {
                if (char.IsLetter(c))
                {
                    char b = char.IsUpper(c) ? 'A' : 'a';
                    char novo = (char)((((c - b + deslocamento) % 26) + 26) % 26 + b);
                    resultado.Append(novo);
                }
                else
                {
                    resultado.Append(c);
                }
            }
            return resultado.ToString();
        }

        static string CesarDecifrar(string texto, int deslocamento)
        {
            return CesarCifrar(texto, -deslocamento);
        }

        static void DemoCesar()
        {
            Separador("CIFRA DE CÉSAR");
            string texto = "mensagem secreta";
            int chave = 3;

            Console.WriteLine($"\n  Texto original : '{texto}'");
            Console.WriteLine($"  Chave          :  {chave} (deslocamento)");

            string cifrado = CesarCifrar(texto, chave);
            Console.WriteLine($"\n  Texto cifrado  : '{cifrado}'");

            string decifrado = CesarDecifrar(cifrado, chave);
            Console.WriteLine($"  Texto decifrado: '{decifrado}'");
            Console.WriteLine($"\n  Verificação: original == decifrado? {texto == decifrado}");
        }

        // ====================================================================
        // SEÇÃO 2 — CIFRA DE VIGENÈRE
        // ====================================================================
        static string Vigenere(string texto, string chave, bool cifrar)
        {
            chave = chave.ToUpper();
            var resultado = new StringBuilder();
            int idxChave = 0;

            foreach (char c in texto)
            {
                if (char.IsLetter(c))
                {
                    char b = char.IsUpper(c) ? 'A' : 'a';
                    int deslocamento = chave[idxChave % chave.Length] - 'A';
                    if (!cifrar) deslocamento = -deslocamento;

                    char novo = (char)((((c - b + deslocamento) % 26) + 26) % 26 + b);
                    resultado.Append(novo);
                    idxChave++;
                }
                else
                {
                    resultado.Append(c);
                }
            }
            return resultado.ToString();
        }

        static void DemoVigenere()
        {
            Separador("CIFRA DE VIGENÈRE");
            string texto = "mensagem secreta";
            string chave = "SEGREDO";

            Console.WriteLine($"\n  Texto original : '{texto}'");
            Console.WriteLine($"  Chave          : '{chave}'");

            string cifrado = Vigenere(texto, chave, true);
            Console.WriteLine($"\n  Texto cifrado  : '{cifrado}'");

            string decifrado = Vigenere(cifrado, chave, false);
            Console.WriteLine($"  Texto decifrado: '{decifrado}'");
            Console.WriteLine($"\n  Verificação: original == decifrado? {texto == decifrado}");
        }

        // ====================================================================
        // SEÇÃO 3 — CIFRA XOR
        // ====================================================================
        static byte[] XorCifrar(string texto, string chave)
        {
            byte[] bytesTexto = Encoding.UTF8.GetBytes(texto);
            byte[] bytesChave = Encoding.UTF8.GetBytes(chave);
            byte[] resultado = new byte[bytesTexto.Length];

            for (int i = 0; i < bytesTexto.Length; i++)
            {
                resultado[i] = (byte)(bytesTexto[i] ^ bytesChave[i % bytesChave.Length]);
            }
            return resultado;
        }

        static string XorDecifrar(byte[] bytesCifrados, string chave)
        {
            byte[] bytesChave = Encoding.UTF8.GetBytes(chave);
            byte[] resultado = new byte[bytesCifrados.Length];

            for (int i = 0; i < bytesCifrados.Length; i++)
            {
                resultado[i] = (byte)(bytesCifrados[i] ^ bytesChave[i % bytesChave.Length]);
            }
            return Encoding.UTF8.GetString(resultado);
        }

        static void DemoXor()
        {
            Separador("CIFRA XOR (SIMÉTRICA POR BITS)");
            string texto = "mensagem secreta";
            string chave = "K9";

            Console.WriteLine($"\n  Texto original : '{texto}'");
            Console.WriteLine($"  Chave          : '{chave}'");

            byte[] bytesCifrados = XorCifrar(texto, chave);
            string hexCifrado = string.Join(" ", bytesCifrados.Select(b => $"{b:X2}"));

            Console.WriteLine($"\n  Bytes cifrados (hex): {hexCifrado}");

            string textoRecuperado = XorDecifrar(bytesCifrados, chave);
            Console.WriteLine($"  Texto decifrado     : '{textoRecuperado}'");
            Console.WriteLine($"\n  Verificação: original == decifrado? {texto == textoRecuperado}");
        }

        // ====================================================================
        // SEÇÃO 4 — FUNÇÃO HASH DIDÁTICA (DJB2)
        // ====================================================================
        static uint HashDjb2(string texto)
        {
            uint hash = 5381;
            foreach (char c in texto)
            {
                hash = ((hash << 5) + hash) + c;
            }
            return hash;
        }

        static void DemoHash()
        {
            Separador("FUNÇÃO HASH DIDÁTICA (DJB2)");
            var entradas = new[] { "mensagem secreta", "Mensagem secreta", "mensagem secreta." };

            Console.WriteLine($"\n  {"Entrada",-24} {"Hash (hex)",12}  {"Hash (decimal)",12}");
            Console.WriteLine($"  {"------------------------",-24} {"------------",12}  {"------------",12}");

            foreach (var e in entradas)
            {
                uint h = HashDjb2(e);
                Console.WriteLine($"  {e,-24} {h,12:X8}  {h,12}");
            }
        }

        // ====================================================================
        // SEÇÃO 5 — MINI RSA
        // ====================================================================
        static BigInteger CalcularInversoModular(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger y = 0, x = 1;

            if (m == 1) return 0;

            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;
                m = a % m;
                a = t;
                t = y;
                y = x - q * y;
                x = t;
            }

            if (x < 0) x += m0;
            return x;
        }

        static void DemoRsa()
        {
            Separador("MINI RSA (CRIPTOGRAFIA ASSIMÉTRICA)");

            BigInteger p = 251;
            BigInteger q = 241;
            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);

            BigInteger e = 7;
            BigInteger d = CalcularInversoModular(e, phi);

            Console.WriteLine($"\n  n (módulo público)  = {n}");
            Console.WriteLine($"  φ(n)                = {phi}");
            Console.WriteLine($"  Chave pública (e)   = {e}");
            Console.WriteLine($"  Chave privada (d)   = {d}");

            string mensagem = "mensagem secreta";
            var listaCifrada = new List<BigInteger>();

            Console.WriteLine($"\n  ── Cifragem da string '{mensagem}' ──");
            foreach (char c in mensagem)
            {
                BigInteger M = new BigInteger((int)c);
                BigInteger C = BigInteger.ModPow(M, e, n);
                listaCifrada.Add(C);
            }

            Console.WriteLine($"  Lista cifrada: [{string.Join(", ", listaCifrada)}]");

            Console.WriteLine($"\n  ── Decifragem ──");
            string recuperado = "";
            foreach (BigInteger C in listaCifrada)
            {
                BigInteger M = BigInteger.ModPow(C, d, n);
                recuperado += (char)(int)M;
            }

            Console.WriteLine($"  Mensagem decifrada : '{recuperado}'");
            Console.WriteLine($"  Verificação        : original == decifrado? {mensagem == recuperado}");
        }
    }
}


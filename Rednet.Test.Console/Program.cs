using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rednet.DataAccess;
using Rednet.Test.Console.Objects;

namespace Rednet.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.InitDatabase();

            Database.VerifyObject<Imovel>();
            Database.VerifyObject<Visitante>();
            Database.VerifyObject<Entrada>();

            Imovel.Truncate();
            Visitante.Truncate();
            Entrada.Truncate();

            using (var _trans = new TransactionObject(false))
            {
                for (int i = 0; i < 20; i++)
                {
                    var _imovel = new Imovel {EnderecoImovel = $"Rua {i}, nº10", Proprietario = $"Proprietário {i}"};
                    _imovel.SaveChanges(transaction: _trans);
                }
                for (int i = 0; i < 20; i++)
                {
                    var _id = i.ZeroEsquerda(2);
                    var _visitante = new Visitante {Nome = $"Nome {_id}"};
                    _visitante.SaveChanges(transaction: _trans);
                }

                var _action = new Action(() =>
                {
                    var _rnd = new Random(1);
                    for (int i = 0; i < 100; i++)
                    {
                        var _yes = _rnd.Next(1, 2) == 1;
                        var _entrada = new Entrada
                        {
                            Autorizador = "Teste",
                            DataHoraEntrada = DateTime.Now,
                            DataHoraSaida = _yes ? DateTime.Now : default(DateTime),
                            IdImovel = _rnd.Next(1, 20),
                            IdVisitante = _rnd.Next(1, 20),
                            MotivoVisita = MotivoVisita.Pessoal,
                            StatusEntrada = _yes ? StatusEntrada.Finalizado : StatusEntrada.Entrada
                        };
                        try
                        {
                            _entrada.SaveChanges(transaction: _trans);
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine(ex.Message);
                        }
                        Thread.Sleep(10);
                    }
                });

                var _watch = new Stopwatch();
                _watch.Start();

                for (int _a = 0; _a < 10; _a++)
                {
                    _action.Invoke();
                    Thread.Sleep(500);
                }
                _watch.Stop();

                System.Console.WriteLine($"Tempo do insert sem transação: {_watch.Elapsed.ToString("g")}");
                _trans.Commit();
            }

            var _date1 = DateTime.Now.Date;
            var _date2 = _date1.AddHours(23).AddMinutes(59).AddSeconds(59);
            var _status = new[] { StatusEntrada.Entrada, StatusEntrada.Finalizado, };


            //var _teste_like_1 = Visitante.Load(v => v.Nome.Contains("04"));
            //var _teste_like_2 = Visitante.Load(v => v.Nome.StartsWith("04"));
            //var _teste_like_3 = Visitante.Load(v => v.Nome.EndsWith("04"));

            //var _entradas_1 = DoQuery<Entrada>(e => e.IdImovel == 1);
            //var _entradas_2 = DoQuery<Entrada>(e => e.IdVisitante == _teste_like_3.IdVisitante);
            var _entradas_3 = DoQuery<Entrada>(e => _status.Contains(e.StatusEntrada) && e.DataHoraEntrada >= _date1 && e.DataHoraEntrada <= _date2);

            System.Console.WriteLine("Pressione qualquer tecla para finalizar...");
            System.Console.Read();
        }

        private static IEnumerable<T> DoQuery<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate = null) where T : DatabaseObject<T>
        {
            var _watch = new Stopwatch();
            _watch.Start();
            var _ret = DatabaseObject<T>.Query(predicate);
            _watch.Stop();

            System.Console.WriteLine($"Tempo da consulta....: {_watch.Elapsed.ToString("g")}");
            System.Console.WriteLine($"Total de registro(s).: {_ret.Count()}");
            //var _trace = Entrada.GetStatementTrace(e => _status.Contains(e.StatusEntrada) && e.DataHoraEntrada >= _date1 && e.DataHoraEntrada <= _date2);

            //System.Console.WriteLine($"SQL:\r\n{_trace}");

            return _ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerClub

{
    internal class Program
    {
        static void Main(string[] args)
        {

        ComputerClub computerClub = new ComputerClub(8);
            computerClub.Work();
        }

    }
    class ComputerClub
    {
        private int _money = 0;
        private List<Computer> _computers = new List<Computer>();
        private Queue<Client> _clients = new Queue<Client>();

        public ComputerClub(int computersCount) 
        {
            Random random = new Random();
            for (int i = 0; i < computersCount; i++)
            {
                _computers.Add(new Computer(random.Next(5, 15)));
            }
            CreateNewClients(25, random);
        }

        public void CreateNewClients(int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(random.Next(100, 251), random));
            }
        }

        public void Work()
        {
            while(_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Баланс компьютурного клуба {_money} RUB. Ждём нового клиента.");
                Console.WriteLine($"У вас новый клиент, и он хочет купить {newClient.DesiredMinutes} минут");
                ShowAllComputesState();

                Console.Write("\nВы предлагаете ему компьютер под номер:");
                string userInput = Console.ReadLine();
                if(int.TryParse(userInput,out int computerNumber))
                {
                    computerNumber -= 1;

                    if (computerNumber >= 0 && computerNumber < _computers.Count)
                    {
                        if (_computers[computerNumber].IsTaken)
                        {
                            Console.WriteLine("Вы пытаетесь посадить клиента, за компьютер, который уже занят. Клиент разозлился и ушёл!");
                        }
                        else
                        {
                            if (newClient.CheckSolvency(_computers[computerNumber]))
                            {
                                Console.WriteLine("Клиент пересчитав деньги, оплатил и сел за компьютер " +  (computerNumber + 1));
                                _money += newClient.Pay();
                                _computers[computerNumber].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("У клиента нехватило денег и он ушёл! ");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Вы сами не знаете за какой компьютер посадить клиента. Он разозлился и ушёл");
                    }
                }
                else
                {
                    CreateNewClients(1, new Random());
                    Console.WriteLine("Неверный ввод! Повторите снова.");
                }

                Console.WriteLine("Что бы перейти к следующему клиенту, нажмите любую клавишу");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }
        }
        private void ShowAllComputesState()
        {
            Console.WriteLine("\nСписок всех компьютеров: ");
            for (int i = 0; i < _computers.Count; i++)
            {
                Console.Write(i + 1 + " - ");
                _computers[i].ShowState();
            }
        }
        private void SpendOneMinute()
        {
            foreach (var computer in _computers)
            {
                computer.SpendOneMinute();
            }
        }
    }
    class Computer
    {
        private Client _client;
        private int _minutesRemeining;
        public bool IsTaken
        {
            get
            {
                return _minutesRemeining > 0;
            }
        }

        public int PricePerMinute {  get;  private set; }

        public Computer( int pricePerMinute)
        {
            PricePerMinute = pricePerMinute;
        }
        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemeining = _client.DesiredMinutes;
        }
        public void BecomeEmpty()
        {
            _client = null;
        }
        public void SpendOneMinute()
        {
            _minutesRemeining--;
        }
        public void ShowState()
        {
            if (IsTaken)
                Console.WriteLine($"Компьютер занят, осталось минут: {_minutesRemeining}");
            else
                Console.WriteLine($"Компьютер свободен, цена за минуту: {PricePerMinute}") ;
        }

    }
    class Client
    {
        private int _money;
        private int _moneyToPay;
        public int DesiredMinutes { get; private set; }


        public Client (int money, Random random)
        {
            _money = money;
            DesiredMinutes = random.Next(10, 30);
        }

        public bool CheckSolvency(Computer computer)
        {
        _moneyToPay = DesiredMinutes * computer.PricePerMinute;
            if (_money > _moneyToPay)
            {
                return true;
            }
            else
            {
                _moneyToPay = 0;
                return false; ;
            }
        }
        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;

        }
    }
}

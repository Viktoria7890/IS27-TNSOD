using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;

namespace KafkaProducer
{
    class UserEventJob
    {
        public int UserId { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public int Rpm { get; set; }
        public DateTime LastSent { get; set; } = DateTime.MinValue;
    }

    public partial class UserEvent
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public override string ToString()
            => $"UserEvent: UserId={UserId}, Endpoint={Endpoint}, Timestamp={Timestamp:yyyy-MM-dd HH:mm:ss}";
    }

    class Program
    {
        private static readonly Dictionary<int, UserEventJob> _jobs = new();
        private static IProducer<Null, string> _producer = null!;
        private const string TopicName = "user-events";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Kafka User Events Producer");

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                ClientId = "user-events-producer",
                Acks = Acks.All,
                EnableIdempotence = true,
                LingerMs = 10,
                BatchSize = 64 * 1024
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };
            
            var sendLoop = RunSendLoop(cts.Token);
            
            await RunCommandLoop(cts.Token);
            
            cts.Cancel();
            await sendLoop;

            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();

            Console.WriteLine("Bye!");
        }

        static async Task RunSendLoop(CancellationToken ct)
        {
            var ticker = new PeriodicTimer(TimeSpan.FromSeconds(1));
            try
            {
                while (await ticker.WaitForNextTickAsync(ct))
                {
                    await SendDueEvents(ct);
                }
            }
            catch (OperationCanceledException) { /* normal shutdown */ }
        }

        static async Task RunCommandLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                Console.WriteLine("\n Доступные команды ");
                Console.WriteLine("1 - Добавить задание");
                Console.WriteLine("2 - Изменить задание");
                Console.WriteLine("3 - Удалить задание");
                Console.WriteLine("4 - Показать все задания");
                Console.WriteLine("5 - Отправить тестовое сообщение");
                Console.WriteLine("0 - Выход");
                Console.Write("Выберите команду: ");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        AddJob();
                        break;
                    case "2":
                        UpdateJob();
                        break;
                    case "3":
                        RemoveJob();
                        break;
                    case "4":
                        ShowJobs();
                        break;
                    case "5":
                        await SendTestMessage();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }
        }

        static void AddJob()
        {
            Console.Write("User ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Ошибка: некорректный User ID");
                return;
            }

            Console.Write("Endpoint: ");
            var endpoint = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                Console.WriteLine("Ошибка: endpoint не может быть пустым");
                return;
            }

            Console.Write("RPM (сообщений в минуту): ");
            if (!int.TryParse(Console.ReadLine(), out int rpm) || rpm <= 0)
            {
                Console.WriteLine("Ошибка: некорректный RPM");
                return;
            }

            _jobs[userId] = new UserEventJob
            {
                UserId = userId,
                Endpoint = endpoint,
                Rpm = rpm
            };

            Console.WriteLine($"Добавлено задание: UserId={userId}, Endpoint={endpoint}, RPM={rpm}");
        }

        static void UpdateJob()
        {
            Console.Write("User ID для изменения: ");
            if (!int.TryParse(Console.ReadLine(), out int userId) || !_jobs.ContainsKey(userId))
            {
                Console.WriteLine("Ошибка: задание не найдено");
                return;
            }

            var job = _jobs[userId];

            Console.Write($"Новый Endpoint [{job.Endpoint}]: ");
            var endpointInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(endpointInput))
                job.Endpoint = endpointInput;

            Console.Write($"Новый RPM [{job.Rpm}]: ");
            var rpmInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(rpmInput) && int.TryParse(rpmInput, out int newRpm) && newRpm > 0)
                job.Rpm = newRpm;

            Console.WriteLine($"Задание обновлено: UserId={userId}, Endpoint={job.Endpoint}, RPM={job.Rpm}");
        }

        static void RemoveJob()
        {
            Console.Write("User ID для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Ошибка: некорректный User ID");
                return;
            }

            if (_jobs.Remove(userId))
                Console.WriteLine($"Задание для UserId={userId} удалено");
            else
                Console.WriteLine("Ошибка: задание не найдено");
        }

        static void ShowJobs()
        {
            if (_jobs.Count == 0)
            {
                Console.WriteLine("Нет активных заданий");
                return;
            }

            Console.WriteLine("\nАктивные задания:");
            foreach (var job in _jobs.Values)
                Console.WriteLine($"UserId: {job.UserId}, Endpoint: {job.Endpoint}, RPM: {job.Rpm}");
        }

        static async Task SendTestMessage()
        {
            var testEvent = new UserEvent
            {
                UserId = 999,
                Endpoint = "TestEndpoint"
            };

            await SendEventToKafka(testEvent);
            Console.WriteLine("Тестовое сообщение отправлено");
        }

        static async Task SendDueEvents(CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            foreach (var job in _jobs.Values)
            {
                var timeSinceLastSent = now - job.LastSent;
                var requiredInterval = TimeSpan.FromMinutes(1.0 / job.Rpm);

                if (timeSinceLastSent >= requiredInterval)
                {
                    var userEvent = new UserEvent
                    {
                        UserId = job.UserId,
                        Endpoint = job.Endpoint,
                        Timestamp = DateTime.UtcNow
                    };

                    await SendEventToKafka(userEvent, ct);
                    job.LastSent = now;

                    Console.WriteLine($"Отправка: {userEvent}");
                }
            }
        }

        static async Task SendEventToKafka(UserEvent userEvent, CancellationToken ct = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(userEvent);

                var dr = await _producer.ProduceAsync(TopicName, new Message<Null, string> { Value = json }, ct);
                Console.WriteLine($"Delivered to {dr.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> ex)
            {
                Console.WriteLine($"Kafka error: {ex.Error.Reason}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки: {ex.Message}");
            }
        }
    }
}

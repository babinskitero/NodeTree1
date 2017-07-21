using Autofac;
using System;
using System.IO;

namespace Tree
{
    class Start
    {
        private static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<NodeTransformer>().As<INodeTransformer>();
            cb.RegisterType<NodeDescriber>().As<INodeDescriber>();
            cb.RegisterType<NodeWriter>().As<INodeWriter>();
            Container = cb.Build();

			//var filePath = args[0];
			var filePath = Directory.GetCurrentDirectory();
			//File.SetAttributes(filePath, FileAttributes.Normal);

			PrintWrite(filePath);
		}

		private static async void PrintWrite(string filePath)
        {
            using(var scope = Container.BeginLifetimeScope())
            {
                var data = new ManyChildrenNode("root",
                    new ManyChildrenNode("child1",
                        new ManyChildrenNode("leaf1"),
                        new ManyChildrenNode("child2",
                            new ManyChildrenNode("leaf2"))));

                var nodeWriter = scope.Resolve<INodeWriter>();
				
				await nodeWriter.WriteToFileAsync(data, filePath);
				
				var result = File.ReadAllText(filePath);
                Console.WriteLine(result);
                Console.ReadKey();
            }
        }
    }
}

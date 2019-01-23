using DeathStar;

namespace DeathStart.UnitTests
{
    /// <summary>
    /// Пример разрыва зависимостей через зазоры (seam)
    /// </summary>
    public class SeamUsingExample
    {
        
        private class ProtectiveFieldGeneratorStub : ProtectiveFieldGenerator
        {
            /// <summary>
            /// Разрыв чере конструктор
            /// </summary>
            /// <param name="reactor"></param>
            public ProtectiveFieldGeneratorStub(IReactor reactor) : base(reactor)
            {
            }

            /// <summary>
            /// С помощью фабрики
            /// </summary>
            public ProtectiveFieldGeneratorStub(ReactorsFactory factory)
            {
                Reactor = factory.Create();
            }

            /// <summary>
            /// Разрыв через переопределение виртуального метода (плохой пример)
            /// </summary>
            /// <returns></returns>
            public override IReactor GetReactor()
            {
                return new Reactor(10);
            }

            /// <summary>
            /// С помощью метода для установки
            /// </summary>
            /// <param name="reactor"></param>
            public void SetReactor(IReactor reactor)
            {
                Reactor = reactor;
            }
            

        }

        /// <summary>
        /// Фабрика по создания реакторов
        /// </summary>
        private class ReactorsFactory
        {
            /// <summary>
            /// Создает новый реактор
            /// </summary>
            /// <returns></returns>
            public IReactor Create()
            {
                return new Reactor(10);
            }
        }
    }

    

}

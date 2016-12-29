using Borg.Infra.CQRS;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Infra.CQRS.Tests.Commands
{
    public class CommandBusTests
    {
        private Mock<IEntity<string>> _entity;
        private Guid _entityKey;
        private Mock<Exception> _exception;
        private string _exceptionText = "mock exception";

        private InMemoryProcessor _inMemoryCommandBus;
        private CommandA _commandA;
        private CommandB _commandB;

        private Mock<IHandlesCommand<CommandA>> _commandAHandler;
        private Mock<IHandlesCommand<CommandB>> _commandBHandler;

        private ICommandResult _commandAResult;
        private ICommandResult _commandBResult;

        public CommandBusTests()
        {
            _entityKey = Guid.NewGuid();
            _entity = new Mock<IEntity<string>>();
            _entity.Setup(x => x.Id).Returns(_entityKey.ToString);
            _exception = new Mock<Exception>();
            _exception.Setup(x => x.ToString()).Returns(_exceptionText);

            _commandA = new CommandA();
            _commandB = new CommandB();

            _commandAResult = CommandResult<IEntity<string>>.Create(true, _entity.Object);
            _commandBResult = CommandResult.Create(_exception.Object);

            _commandAHandler = new Mock<IHandlesCommand<CommandA>>();
            _commandAHandler.Setup(x => x.Execute(_commandA)).ReturnsAsync(_commandAResult);
            _commandBHandler = new Mock<IHandlesCommand<CommandB>>();
            _commandBHandler.Setup(x => x.Execute(_commandB)).ReturnsAsync(_commandBResult);
        }

        [Fact]
        public void check_that_we_can_register_command_handlers_to_the_in_memory_processor()
        {
            Should.NotThrow(() =>
            {
                _inMemoryCommandBus = new InMemoryProcessor();
                Func<CommandA, IResponse> a = command => _commandAHandler.Object.Execute(_commandA).Result;
                Func<CommandB, IResponse> b = command => _commandBHandler.Object.Execute(_commandB).Result;
                _inMemoryCommandBus.RegisterHandler(a);
                _inMemoryCommandBus.RegisterHandler(b);
            });
        }

        [Fact]
        public void check_that_multiple_handlers_for_command_throw()
        {
            _inMemoryCommandBus = new InMemoryProcessor();
            Func<CommandA, IResponse> a = command => _commandAHandler.Object.Execute(_commandA).Result;
            Func<CommandA, IResponse> b = command => _commandBHandler.Object.Execute(_commandB).Result;
            _inMemoryCommandBus.RegisterHandler(a);
            _inMemoryCommandBus.RegisterHandler(b);

            Should.Throw<MultipleHandlersForCommandException>(async () =>
            {
                await _inMemoryCommandBus.Process(_commandA);
            });
        }

        [Fact]
        public void check_that_no_handlers_for_command_throw()
        {
            _inMemoryCommandBus = new InMemoryProcessor();
            Func<CommandA, IResponse> a = command => _commandAHandler.Object.Execute(_commandA).Result;
            Func<CommandA, IResponse> b = command => _commandBHandler.Object.Execute(_commandB).Result;
            _inMemoryCommandBus.RegisterHandler(a);
            _inMemoryCommandBus.RegisterHandler(b);

            Should.Throw<NoHandlersForCommandException>(async () =>
            {
                await _inMemoryCommandBus.Process(_commandB);
            });
        }

        [Fact]
        public async Task check_that_a_handler_executes()
        {
            _inMemoryCommandBus = new InMemoryProcessor();
            Func<CommandA, IResponse> a = command => _commandAHandler.Object.Execute(_commandA).Result;
            Func<CommandB, IResponse> b = command => _commandBHandler.Object.Execute(_commandB).Result;
            _inMemoryCommandBus.RegisterHandler(a);
            _inMemoryCommandBus.RegisterHandler(b);

            var resultA = await _inMemoryCommandBus.Process(_commandA);
            resultA.ShouldNotBeNull();
            resultA.Success.ShouldBe(true);

            var typedResA = resultA as ICommandResult<IEntity<string>>;
            typedResA.ShouldNotBeNull();
            typedResA.Entity.ShouldNotBeNull();
            typedResA.Entity.Id.ShouldBe(_entityKey.ToString());

            var resultB = await _inMemoryCommandBus.Process(_commandB);
            resultB.ShouldNotBeNull();
            resultB.Success.ShouldBe(false);
            resultB.Description.ShouldBe(_exceptionText);
        }

        [Fact]
        public async Task check_that_when_a_handler_executes_results_in_command_result()
        {
            _inMemoryCommandBus = new InMemoryProcessor();
            Func<CommandA, IResponse> a = command => _commandAHandler.Object.Execute(_commandA).Result;
            Func<CommandB, IResponse> b = command => _commandBHandler.Object.Execute(_commandB).Result;
            _inMemoryCommandBus.RegisterHandler(a);
            _inMemoryCommandBus.RegisterHandler(b);

            var resultA = await _inMemoryCommandBus.Process(_commandA);
            resultA.ShouldNotBeNull();
            resultA.Success.ShouldBe(true);
            resultA.GetType().GetInterfaces().ShouldContain(typeof(ICommandResult));
            resultA.GetType().GetInterfaces().ShouldContain(typeof(ICommandResult<IEntity<string>>));

            var resultB = await _inMemoryCommandBus.Process(_commandB);
            resultB.ShouldNotBeNull();
            resultB.Success.ShouldBe(false);
            resultB.GetType().GetInterfaces().ShouldContain(typeof(ICommandResult));
        }

        public class CommandA : Borg.Infra.CQRS.ICommand
        {
        }

        public class CommandB : Borg.Infra.CQRS.ICommand
        {
        }
    }
}
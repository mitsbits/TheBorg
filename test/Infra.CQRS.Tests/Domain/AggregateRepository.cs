﻿using Borg.Infra;
using Borg.Infra.CQRS;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Infra.CQRS.Tests.Domain
{
    public class AggregateRepositoryTests
    {
        private IDomainEventStore<string> _inMemoryEventStore;
        private IEventBus _inMemoryPublisher;
        private ICommandBus _inMemoryProcessor;
        private IAggregateRepository<string> _repository;

        public AggregateRepositoryTests()
        {
            _inMemoryPublisher = new InMemoryPublisher();
            _inMemoryEventStore = new InMemoryEventStore<string>(_inMemoryPublisher);
            _inMemoryProcessor = new InMemoryProcessor();
            _repository = new AggregateRepository<string>(_inMemoryEventStore, _inMemoryPublisher);

            DummyAggregateCommandHandler commandHandler = new DummyAggregateCommandHandler(_repository);
            ((InMemoryProcessor)_inMemoryProcessor).RegisterHandler<CreateDummyAggregateCommand>((c) => AsyncHelpers.RunSync(() => commandHandler.Execute(c)));
            ((InMemoryProcessor)_inMemoryProcessor).RegisterHandler<ChangeDummyAggregateNameCommand>((c) => AsyncHelpers.RunSync(() => commandHandler.Execute(c)));
            ((InMemoryPublisher)_inMemoryPublisher).RegisterHandler<DummyAggregateCreatedEvent>((e) => (new DummyAggregateCreatedEventHandler()).Handle(e));
            ((InMemoryPublisher)_inMemoryPublisher).RegisterHandler<DummyAggregateNameChangedEvent>((e) => (new DummyAggregateNameChangedEventHandler()).Handle(e));
        }

        [Fact]
        public async Task test()
        {
            await _inMemoryProcessor.Process(new CreateDummyAggregateCommand() { Id = "xxx-ooooo-xx-ooo", Name = "test" });
            await _inMemoryProcessor.Process(new ChangeDummyAggregateNameCommand() { Id = "xxx-ooooo-xx-ooo", Name = "test 2" });
        }

        private class DummyAggregate : AggregateRoot<string>
        {
            protected DummyAggregate()
            {
            }

            public DummyAggregate(string id, string name)
            {
                ApplyChange(new DummyAggregateCreatedEvent(id, name));
            }

            public void SetName(string name)
            {
                if (name == Name) return;
                ApplyChange(new DummyAggregateCreatedEvent(Id, name));
            }

            private void Apply(DummyAggregateNameChangedEvent @event)
            {
                Name = @event.Name;
            }

            private void Apply(DummyAggregateCreatedEvent @event)
            {
                SetId(@event.Id);
                Name = @event.Name;
            }

            public string Name { get; protected set; }
        }

        private class CreateDummyAggregateCommand : ICommand
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        private class ChangeDummyAggregateNameCommand : ICommand
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        private class DummyAggregateCommandHandler : IHandlesCommand<CreateDummyAggregateCommand>, IHandlesCommand<ChangeDummyAggregateNameCommand>
        {
            public async Task<ICommandResult> Execute(CreateDummyAggregateCommand message)
            {
                var entity = new DummyAggregate(message.Id, message.Name);
                await _repository.Save(entity);
                return CommandResult<DummyAggregate>.Create(true, entity);
            }

            private IAggregateRepository<string> _repository;

            public DummyAggregateCommandHandler(IAggregateRepository<string> repository)
            {
                _repository = repository;
            }

            public async Task<ICommandResult> Execute(ChangeDummyAggregateNameCommand message)
            {
                var entity = await _repository.Get<DummyAggregate>("xxx-ooooo-xx-ooo");
                entity.SetName(message.Name);
                await _repository.Save(entity);
                return CommandResult<DummyAggregate>.Create(true, entity);
            }
        }

        private class DummyAggregateCreatedEvent : DomainEvent<string>
        {
            public DummyAggregateCreatedEvent(string id, string name) : base(id)
            {
                Name = name;
            }

            public string Name { get; set; }
        }

        private class DummyAggregateNameChangedEvent : DomainEvent<string>
        {
            public DummyAggregateNameChangedEvent(string id, string name) : base(id)
            {
                Name = name;
            }

            public string Name { get; set; }
        }

        private class DummyAggregateCreatedEventHandler : Borg.Infra.CQRS.EventHandler<DummyAggregateCreatedEvent>
        {
            public string Name { get; set; }

            public override Task Handle(DummyAggregateCreatedEvent message)
            {
                Console.WriteLine($"{nameof(DummyAggregateCreatedEventHandler)} fired");
                return Task.CompletedTask;
            }
        }

        private class DummyAggregateNameChangedEventHandler : Borg.Infra.CQRS.EventHandler<DummyAggregateNameChangedEvent>
        {
            public string Name { get; set; }

            public override Task Handle(DummyAggregateNameChangedEvent message)
            {
                Console.WriteLine($"{nameof(DummyAggregateNameChangedEventHandler)} fired");
                return Task.CompletedTask;
            }
        }
    }
}
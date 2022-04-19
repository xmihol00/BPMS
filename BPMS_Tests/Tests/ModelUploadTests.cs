using Xunit;
using BPMS_DAL;
using BPMS_Tests.Factories;
using System;
using BPMS_BL.Facades;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using System.Threading.Tasks;
using System.IO;
using BPMS_DAL.Entities;

namespace BPMS_Tests.Tests
{
    public class ModelUploadTests
    {
        private readonly ModelUploadFacade _facade;
        private readonly ModelRepository _modelRepository;

        public ModelUploadTests()
        {
            BpmsDbContext context = DbContextInMemoryFactory.CreateDbContext(Guid.NewGuid().ToString());
            _modelRepository = new ModelRepository(context);
            _facade = new ModelUploadFacade(_modelRepository, new PoolRepository(context), new BlockModelRepository(context),
                                            new LaneRepository(context), new AgendaRepository(context), new SolvingRoleRepository(context),
                                            new AgendaRoleRepository(context), new FilterRepository(context));
        }

        [Fact]
        public async Task CorrectUpload1()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test1", "test1", "single_message.bpmn", "single_message.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }

        [Fact]
        public async Task CorrectUpload2()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test2", "test2", "three_pools.bpmn", "three_pools.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }

        [Fact]
        public async Task CorrectUpload3()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test3", "test3", "message_and_service.bpmn", "message_and_service.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }

        [Fact]
        public async Task CorrectUpload4()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test4", "test4", "only_user_task.bpmn", "only_user_task.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }

        [Fact]
        public async Task CorrectUpload5()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test5", "test5", "service_and_task.bpmn", "service_and_task.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }

        [Fact]
        public async Task CorrectUpload6()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test6", "test6", "signal_event1.bpmn", "signal_event1.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }

        [Fact]
        public async Task CorrectUpload7()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test7", "test7", "signal_event2.bpmn", "signal_event2.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }

        [Fact]
        public async Task CorrectUpload8()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test8", "test8", "double_service.bpmn", "double_service.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }
        
        [Fact]
        public async Task CorrectUpload9()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test9", "test9", "signal_and_service.bpmn", "signal_and_service.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }
        
        [Fact]
        public async Task CorrectUpload10()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test10", "test10", "message_and_signal.bpmn", "message_and_signal.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }

        [Fact]
        public async Task CorrectUpload11()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test11", "test11", "user_testing.bpmn", "user_testing.svg");
            Guid result = await _facade.Upload(dto);
            
            Assert.NotEqual(result, Guid.Empty);
            Assert.True(await _modelRepository.Any(result));
        }

        [Fact]
        public async Task FileMissmatch()
        {
            ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "test3", "test3", "signal_event1.bpmn", "signal_event2.svg");

            try
            {
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("SVG soubor neodpovídá BPMN souboru.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task FileNotUploaded()
        {
            ModelCreateDTO dto = new ModelCreateDTO();

            try
            {
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("BPMN soubor nebyl nahrán.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task UnlinkedMessageEvent1()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/unlinked_message_event1.bpmn", "Incorrect/unlinked_message_event1.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Události typu 'Message Event' musí být spojeny elementem 'Message Flow'.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task UnlinkedMessageEvent2()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/unlinked_message_event2.bpmn", "Incorrect/unlinked_message_event2.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Události typu 'Message Event' musí být spojeny elementem 'Message Flow'.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task NoPool()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/no_pool.bpmn", "Incorrect/no_pool.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Model obsahuje elementy mimo element 'Pool'.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task UnnamedPool()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/unnamed_pool.bpmn", "Incorrect/unnamed_pool.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Model obsahuje nepojmenovaný bazén.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task UnnamedLane()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/unnamed_lane.bpmn", "Incorrect/unnamed_lane.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Model obsahuje nepojmenovanou dráhu.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task UnnamedBlock()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/unnamed_block.bpmn", "Incorrect/unnamed_block.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Model obsahuje nepojmenovaný blok.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task UnknownBlock1()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/unknown_block1.bpmn", "Incorrect/unknown_block1.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Nepodporaovaný BPMN prvek.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task UnknownBlock2()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/unknown_block2.bpmn", "Incorrect/unknown_block2.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Nepodporaovaný BPMN prvek.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task UnknownBlock3()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/unknown_block3.bpmn", "Incorrect/unknown_block3.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Nepodporaovaný BPMN prvek.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task NoStartEvent()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/no_start_event.bpmn", "Incorrect/no_start_event.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Blok typu 'User Task' musí mít právě jeden příchozí a maximálně jeden maximálně jeden odchozí řídící tok.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task TaskBranching()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/task_branching.bpmn", "Incorrect/task_branching.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Blok typu 'User Task' musí mít právě jeden příchozí a maximálně jeden maximálně jeden odchozí řídící tok.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task NoEndEvent()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/no_end_event.bpmn", "Incorrect/no_end_event.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Větev není možné úspěšně vykonat.", e.Message);
                return;
            }
            
            Assert.True(false);
        }

        [Fact]
        public async Task Loop()
        {
            try
            {
                ModelCreateDTO dto = ModelCreateDTOFactory.Create(Guid.NewGuid(), "", "", "Incorrect/loop.bpmn", "Incorrect/loop.svg");
                Guid result = await _facade.Upload(dto);
            }
            catch (ParsingException e)
            {
                Assert.Equal("Blok typu 'User Task' musí mít právě jeden příchozí a maximálně jeden maximálně jeden odchozí řídící tok.", e.Message);
                return;
            }
            
            Assert.True(false);
        }
    }
}

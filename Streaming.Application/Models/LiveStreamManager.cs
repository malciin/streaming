using Autofac;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Live;
using Streaming.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Streaming.Application.Models
{
    public class LiveStreamManager : ILiveStreamManager
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly Mapper mapper;
        private readonly ConcurrentDictionary<Guid, LiveStream> streams;

        public LiveStreamManager(ILifetimeScope lifetimeScope, Mapper mapper)
        {
            streams = new ConcurrentDictionary<Guid, LiveStream>();
            this.lifetimeScope = lifetimeScope;
            this.mapper = mapper;
        }

        public async Task StartNewLiveStreamAsync(NewLiveStreamDTO newLiveStream)
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                var liveStreams = scope.Resolve<IPastLiveStreamRepository>();

                var lastLiveStream = (await liveStreams.GetAsync(x => x.Owner.UserId == newLiveStream.User.UserId))
                    .Items.OrderByDescending(x => x.Ended).FirstOrDefault();

                streams.TryAdd(newLiveStream.LiveStreamId, new LiveStream
                {
                    Title = lastLiveStream?.Title ?? "Untitled",
                    LiveStreamId = newLiveStream.LiveStreamId,
                    Owner = newLiveStream.User,
                    Started = DateTime.UtcNow,
                    ManifestUrl = newLiveStream.ManifestUri
                });
            }
        }

        public async Task FinishLiveStreamAsync(Guid streamId)
        {
            streams.TryRemove(streamId, out LiveStream pastLiveStream);
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                var liveStreamRepo = scope.Resolve<IPastLiveStreamRepository>();

                pastLiveStream.Ended = DateTime.UtcNow;

                await liveStreamRepo.AddAsync(pastLiveStream);
            }
        }

        public LiveStreamMetadataDTO GetSingle(Guid liveStreamId)
        {
            if (liveStreamId == Guid.Parse("680cf699-7425-4e16-ab4e-65baa346d00c"))
                return new LiveStreamMetadataDTO
                {
                    Title = "Stream title",
                    LiveStreamId = Guid.Parse("680cf699-7425-4e16-ab4e-65baa346d00c"),
                    ManifestUrl =
                        "https://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8",
                    Started = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30)),
                    UserStarted = "malciin"
                };
            else
                return mapper.MapLiveStreamMetadataDTO(streams[liveStreamId]);
        }

        public IEnumerable<LiveStreamMetadataDTO> Get(EnumerableFilter<LiveStream> filter)
        {
            return filter(streams.Values).Select(x => mapper.MapLiveStreamMetadataDTO(x)).Concat(new List<LiveStreamMetadataDTO>
            {
                new LiveStreamMetadataDTO
                {
                    Title = "Stream title",
                    LiveStreamId = Guid.Parse("680cf699-7425-4e16-ab4e-65baa346d00c"),
                    ManifestUrl = "https://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8",
                    Started = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30)),
                    UserStarted = "malciin"
                }
            });
        }

        public async Task<PastLiveStreamMetadataDTO> GetPastSingleAsync(Expression<Func<LiveStream, bool>> filter)
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                var pastLiveStreams = scope.Resolve<IPastLiveStreamRepository>();
                return mapper.MapPastLiveStreamMetadataDTO(await pastLiveStreams.GetSingleAsync(filter));
            }
        }

        public async Task<IEnumerable<PastLiveStreamMetadataDTO>> GetPastAsync(Expression<Func<LiveStream, bool>> filter, int skip, int limit)
        {
            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                var pastLiveStreams = scope.Resolve<IPastLiveStreamRepository>();
                return (await pastLiveStreams.GetAsync(filter, skip, limit))
                    .Items.Select(x => mapper.MapPastLiveStreamMetadataDTO(x));
            }
        }
    }
}

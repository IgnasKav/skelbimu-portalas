using Application.BackgroundJobs;

namespace API.DTOs;

public class BackgroundJobRequest
{
    public ElasticBackgroundOperation operation { get; set; }
}
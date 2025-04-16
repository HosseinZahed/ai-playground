using Microsoft.SemanticKernel.ChatCompletion;

namespace Playground.Infrastructure.Services;

public interface IChatServiceBase
{
    Task<string?> GetChatResponseAsync(ChatHistory chatHistory,
        CancellationToken cancellationToken);

    IAsyncEnumerable<string?> GetChatStreamingResponseAsync(ChatHistory chatHistory,
        CancellationToken cancellationToken);
}

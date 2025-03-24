﻿

using Newtonsoft.Json;

namespace EPSPlus.Domain.Responses;

public class ServerResponse<T>
{
    public ServerResponse(bool success = false)
    {
        IsSuccessful = success;
        Data = default!;
    }

    public bool IsSuccessful { get; set; }
    public ErrorResponse ErrorResponse { get; set; }

    [JsonProperty("responseCode")]
    public string ResponseCode { get; set; }

    [JsonProperty("responseMessage")]
    public string ResponseMessage { get; set; }

    public T Data { get; set; }
}

public class ErrorResponse
{
    [JsonProperty("responseCode")]
    public string? ResponseCode { get; set; }

    [JsonProperty("responseMessage")]
    public string? ResponseMessage { get; set; }

    [JsonProperty("responseDescription")]
    public string? ResponseDescription { get; set; }
}


public static class ServerResponseExtensions
{
    public static ServerResponse<T> Failure<T>(ErrorResponse error, int statusCode)
    {
        return new ServerResponse<T>
        {
            IsSuccessful = false,
            ResponseCode = statusCode.ToString(),
            ResponseMessage = "Error occurred",
            ErrorResponse = error
        };
    }
}
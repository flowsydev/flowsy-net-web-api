using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Flowsy.Mediation;
using Flowsy.Web.Api.Security;
using MediatR;

namespace Flowsy.Web.Api.Mediation;

[Serializable]
public class ApiRequest<TResult> : Request<TResult>
{
    /// <summary>
    /// The current API client.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public ApiClient Client { get; internal set; } = ApiClient.Current;
}

public class ApiRequest : ApiRequest<Unit>
{
}
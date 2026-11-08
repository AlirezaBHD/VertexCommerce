using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Notifications.Endpoints;

public sealed record PushSubscribeRequest(
    string Endpoint,
    string P256dh,
    string Auth,
    string? UserAgent
);

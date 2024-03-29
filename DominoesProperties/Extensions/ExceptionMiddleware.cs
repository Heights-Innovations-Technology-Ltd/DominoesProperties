﻿using System;
using System.Net;
using System.Threading.Tasks;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Repositories.Repository;

namespace DominoesProperties.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly ILoggerManager _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var message = exception switch
            {
                BadHttpRequestException => "Invalid request payload supplied",
                NotImplementedException => "Method not implemented in logic",
                UnauthorizedAccessException => "User does not have required permission to access this endpoint",
                DbUpdateException => exception.Message,
                MySqlException => "Error performing operation, kindly try again later or contact admin for support",
                _ => exception.Message
            };

            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            }.ToString());
        }
    }
}
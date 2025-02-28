using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Authorize]
public class MessagesController(IMapper mapper, IUnitOfWork unitOfWork) : BaseApiController
{

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> AddMessage(CreateMessageDTO createMessageDTO)
    {
        var username = User.GetUsername();
        if (username == createMessageDTO.RecipientUsername.ToLower()) return BadRequest("You cant message yourself");

        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername.ToLower());

        if (sender == null || recipient == null ||
        sender.UserName == null || recipient.UserName == null) return BadRequest("Either the sender or recipient does not exist");

        var newMessage = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content,
        };
        unitOfWork.MessagesRepository.Add(newMessage);
        if (await unitOfWork.Complete()) return Ok(mapper.Map<MessageDTO>(newMessage));

        return BadRequest("Something went wrong. Could not add a new message!");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var currentUserUsername = User.GetUsername();
        var existingMessage = await GetMessageAsync(id);
        if (existingMessage == null) return BadRequest("Could not find message!");
        if (existingMessage.SenderUsername != currentUserUsername && existingMessage.RecipientUsername != currentUserUsername)
            return Forbid();
        if (existingMessage.SenderUsername == currentUserUsername)
            existingMessage.SenderDeleted = true;
        if (existingMessage.RecipientUsername == currentUserUsername)
            existingMessage.RecipientDeleted = true;
        if (existingMessage is { SenderDeleted: true, RecipientDeleted: true })
            unitOfWork.MessagesRepository.Delete(existingMessage);
        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("Could not successfully delete the message!");
    }

    [HttpGet("{id:int}")]
    public async Task<Message?> GetMessageAsync(int id)
    {
        return await unitOfWork.MessagesRepository.GetMessageAsync(id);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUserAsync([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        if (messageParams.Username == null) return BadRequest("Current user not found");
        var messages = await unitOfWork.MessagesRepository.GetMessagesForUserAsync(messageParams);
        if (messages == null) return NotFound();
        Response.AddPaginationHeader(messages);
        return messages;
    }
    [HttpGet("thread/{recipientUsername}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string recipientUsername)
    {
        var currentUsername = User.GetUsername();
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(recipientUsername);
        if (currentUsername == null || recipient == null || recipient.UserName == null) return NotFound();
        recipientUsername = recipient.UserName;
        return Ok(await unitOfWork.MessagesRepository.GetMessageThread(currentUsername, recipientUsername));
    }
}
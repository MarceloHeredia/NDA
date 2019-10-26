using System;
using System.Linq;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

namespace NDA.CustomPreconditions
{
    /// <summary>Require the user to be on a specified role to use the command</summary>
    public class RequireRoleAttribute : PreconditionAttribute
    {
        //create a field to store the specified name
        private readonly String _name;

        //create a constructor so the name can be specified
        public RequireRoleAttribute(String name) => _name = name;

        /// <summary>Override the checkPermissions method</summary>
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, 
                                                                                                            CommandInfo command,
                                                                                                            IServiceProvider services)
        {
            //check if this user is a guild user, wich is the only context where roles exist
            if (context.User is SocketGuildUser gUser)
            {
                // if this command was executed by a user with the appropriate role, return a success
                if (gUser.Roles.Any(r => r.Name == _name))
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromError($"Só membros com cargo {_name} podem executar esse comando."));
                }
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command."));
            }
        }
    }
}

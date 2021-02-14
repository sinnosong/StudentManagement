using Microsoft.AspNetCore.Identity;

namespace StudentManagement.CustomerMiddlewares
{
    public class CustomerIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError { Code = nameof(DefaultError), Description = "发生了未知的故障" };
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "乐观并发失败，对象已被修改" };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError { Code = nameof(PasswordMismatch), Description = "密码错误" };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError { Code = nameof(InvalidToken), Description = "无效令牌" };
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "登录已关联" };
        }

        public override IdentityError InvalidRoleName(string name)
        {
            return new IdentityError { Code = nameof(InvalidRoleName), Description = $"用户名{name}不合法，只能包含字母和数字" };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError { Code = nameof(InvalidEmail), Description = $"邮箱{email}不合法" };
        }

        public override IdentityError DuplicateUserName(string name)
        {
            return new IdentityError { Code = nameof(DuplicateUserName), Description = $"用户名{name}无效" };
        }
    }
}
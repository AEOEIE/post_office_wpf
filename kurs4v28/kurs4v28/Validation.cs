using kurs4v28.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace kurs4v28
{
    public static class Validation
    {
        //для CreateCkientWindow
        public static (bool isValid, string error) CreateClientValidation(string clphone, string clpassport, post_officeEntities2 context = null)
        {
            context = context ?? new post_officeEntities2();
            if (context.Client.Any(c => c.phone == clphone))
            {
                return (false, "Клиент с таким номером телефона уже существует!");
            }
            else if (context.Client.Any(c => c.passport_seria_number == clpassport))
            {
                return (false, "Клиент с такими паспортными данными уже существует!");
            }
            else { return (true, ""); }
        }
        public static (bool isValid, string error) EditClientValidation(string clphone, string clpassport, Client clientt, post_officeEntities2 context = null)
        {
            context = context ?? new post_officeEntities2();
            if (context.Client.Any(c => c.phone == clphone && c.id != clientt.id))
                return (false, "Клиент с таким номером телефона уже существует!");

            if (context.Client.Any(c => c.passport_seria_number == clpassport && c.id != clientt.id))
                return (false, "Клиент с такими паспортными данными уже существует!");

            return (true, "");
        }
        // Для CreateServiceWindow
        public static (bool isValid, string error) CreateServiceValidation(string servName, post_officeEntities2 context = null)
        {
            context = context ?? new post_officeEntities2();
            if (context.services.Any(s => s.name == servName))
                return (false, "Такая услуга уже существует!");

            return (true, "");
        }
        public static (bool isValid, string error) EditServiceValidation(string servName, services servicee, post_officeEntities2 context = null)
        {
            context = context ?? new post_officeEntities2();
            if (context.services.Any(s => s.name == servName && s.id != servicee.id))
                return (false, "Такая услуга уже существует!");

            return (true, "");
        }
        // Для CreateSposobWindow
        public static (bool isValid, string error) EditSposobValidation(string sposName, Sposob_otpravki sp, post_officeEntities2 context = null)
        {
            context = context ?? new post_officeEntities2();
            if (context.Sposob_otpravki.Any(s => s.name == sposName && s.id != sp.id))
                return (false, "Такой способ доставки уже существует!");

            return (true, "");
        }

        public static (bool isValid, string error) CreateSposobValidation(string sposName, post_officeEntities2 context = null)
        {
            context = context ?? new post_officeEntities2();
            if (context.Sposob_otpravki.Any(s => s.name == sposName))
                return (false, "Такой способ доставки уже существует!");

            return (true, "");
        }
        // Для CreateUserWindow
        public static (bool isValid, string error) CreateUserValidation(string login, string pas, post_officeEntities2 context = null)
        {
            context = context ?? new post_officeEntities2();
            if (context.employee.Any(d => d.login == login))
                return (false, "Пользователь с таким логином уже существует!");
            if (pas.Length < 8)
            {
                return (false, "Должно быть не меньше 8 символов");
            }
            int kolnum = 0;
            int kolletter = 0;
            foreach (char p in pas) { if (char.IsDigit(p)) { kolnum++; } if (char.IsLetter(p)) { kolletter++; } }
            if (kolnum == 0) { return (false, "Пароль должен содержать хотя бы одну цифру!"); }
            if (kolletter == 0) { return (false, "Пароль должен содержать хотя бы одну букву!"); }
            return (true, "");
        }
        public static (bool isValid, string error) EditUserValidation(string login, string pas, Role current_role, employee emp, post_officeEntities2 context = null)
        {
            context = context ?? new post_officeEntities2();
            if (context.employee.Any(d => d.login == login && d.id != emp.id))
                return (false, "Пользователь с таким логином уже существует!");
            if ((pas.Length < 8)&&pas.Length>0)
            {
                return (false, "Должно быть не меньше 8 символов");
            }
            if(pas.Length > 0)
            {
                int kolnum = 0;
                int kolletter = 0;
                foreach (char p in pas) { if (char.IsDigit(p)) { kolnum++; } if (char.IsLetter(p)) { kolletter++; } }
                if (kolnum == 0) { return (false, "Пароль должен содержать хотя бы одну цифру!"); }
                if (kolletter == 0) { return (false, "Пароль должен содержать хотя бы одну букву!"); }
            }
            if (Pass.VerifyBcryptPassword(pas, emp.password)) { return (false, "Пароль не может быть похож на старый!"); }
            if (emp.role_id == 1)
            {
                if (current_role.id != 1)
                    return (false, "Вы не можете менять роль админа!");
            }
            else
            {
                if (current_role.id == 1)
                    return (false, "Вы не можете сделать этого пользователя админом!");
            }
            return (true, "");
        }

    }
}

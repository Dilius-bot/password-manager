# Менеджер Паролей

Простая и надежная программа на C# для хранения паролей. Главная особенность — ваши данные зашифрованы так, что их не прочитает никто, кроме вас.

### Что умеет программа:

-   Пароли хранятся в файле в виде «каши» из символов.
-   При входе вы создаете главный ключ. Без него базу не открыть.
-   Когда вы печатаете пароли, на экране отображаются только звездочки `*`.
-   Можно добавлять, искать и удалять записи кнопками 1-6.

### Как это работает внутри:

-   Все данные аккуратно разложены по полочкам в текстовом файле.
-   Ваш главный пароль не хранится в программе. Он превращается в секретный ключ прямо в оперативной памяти.

### Как пользоваться:

1. Запустите программу.
2. Придумайте и подтвердите МАСТЕР-ПАРОЛЬ (не забудьте его, восстановить нельзя!).
3. Используйте меню для управления своими паролями.
4. Редактируйте и удаляйте файл с паролями только через программу.

# Password Manager

A simple and reliable C# program for storing passwords. The main feature — your data is encrypted so that no one but you can read it.

### What the program can do:

-   Passwords are stored in a file as a "scramble" of characters.
-   You create a master key upon entry. Without it, the database cannot be opened.
-   When you type passwords, only asterisks `*` are displayed on the screen.
-   You can add, search, and delete entries using buttons 1-6.

### How it works inside:

-   All data is neatly organized in a text file.
-   Your master password is not stored in the program. It turns into a secret key directly in the RAM.

### How to use:

1. Run the program.
2. Create and confirm a MASTER PASSWORD (do not forget it, it cannot be recovered!).
3. Use the menu to manage your passwords.
4. Edit and delete the password file only through the program.

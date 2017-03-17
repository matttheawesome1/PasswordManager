using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    /*
     * Copyright (c) 2006 Damien Miller <djm@mindrot.org> (jBCrypt)
     * Copyright (c) 2013 Ryan D. Emerle (.Net port)
     * Permission to use, copy, modify, and distribute this software for any
     * purpose with or without fee is hereby granted, provided that the above
     * copyright notice and this permission notice appear in all copies.
     * 
     * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
     * WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
     * MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
     * ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
     * WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
     * ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
     * OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
     */

    using BCrypt.Net; //Hashing algorithm library by Damien Miller and Ryan D Emerle.
    
    /// <summary>
    /// This is a class that stores functions that generate a random salt, hash the password using that salt,
    /// and then a function that verifies the password by using the hash stored in the directory.
    /// </summary>
    public class Hashing
    {
        /// <summary>
        /// Simply put, generates a random salt with a work factor of 2 to the power of x.
        /// </summary>
        /// <returns>Returns salt with given workfactor.</returns>
        private static string GetRandomSalt()
        {
            return BCrypt.GenerateSalt(12);
        }

        /// <summary>
        /// Hash the password by using a salt from the GetRandomSalt function.
        /// </summary>
        /// <param name="password">The string a user enters into the form for MasterPassword</param>
        /// <returns>Returns the hashed password.</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.HashPassword(password, GetRandomSalt());
        }

        /// <summary>
        /// Validate the password by checking if the hash of the entered password is the same as the password stored on file (the file is encrypted).
        /// </summary>
        /// <param name="password"></param>
        /// <param name="correctHash"></param>
        /// <returns>Returns true or false on whether or not the password given was valid.</returns>
        public static bool ValidatePassword(string password, string correctHash)
        {
            return BCrypt.Verify(password, correctHash);
        }
    }
}
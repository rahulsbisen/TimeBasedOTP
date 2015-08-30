# TimeBasedOTP
Time Based One Time Password Generation &amp; Authentication

Problem : Write a program or API that can generate a one-time password and verify if one password it is valid for one user. The input of the program should be the: User Id to generate the password ad the User Id and the password to verify the valid of the password. Every generated password should be valid for 30 seconds.

You are free to use a Web, MVC, Console or Class Library project in order to accomplish the requirement.

Your solution will be evaluated based on coding standards, naming conventions, project structure and the meeting of requirements. The use of unit testing is highly recommended. 

Please submit your solution to us in a ZIP file by email or with a link to the solution in github. Do not include compiled objects or third party DLLs if you are sending the solution thought email.


Solution :

Library implements an interface IOTPService which provides OTP generation and validation.

OTPConfiguration exposes configuration values which impact the algorithm.

1. OTPExpiryInSeconds -> Configure after how many seconds should OTP expire. Default : 30 sec
2. PrivateKey -> Hidden key which is not exposed to the client, helps in decreasing the chance of attacker determing the algorithm.
3. NumberOfDigitsInOTP -> Number of digits to generate, currently limited between 1 and 8 inclusive. Default : 6


Client:

Generate OTP snippet:

<pre><code>
 var generateOTPResponse = otpService.GenerateOtp(new GenerateOTPRequest()
            {
                UserId = userId
            });
Console.WriteLine(generateOTPResponse.OTP);
</code></pre>

Validate OTP snippet:

<pre><code>
 var validateOTPResponse = otpService.ValidateOtp(new ValidateOTPRequest()
            {
                OTP = userInputOtp,
                UserId = userId
            });
Console.WriteLine(validateOTPResponse.Success);
</code></pre>

Algorithm Reference : https://tools.ietf.org/html/rfc4226

<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use App\Models\ClientConfig;
use PHPMailer\PHPMailer\Exception;
use PHPMailer\PHPMailer\OAuth;
use PHPMailer\PHPMailer\SMTP;
use PHPMailer\PHPMailer\PHPMailer;
use League\OAuth2\Client\Provider\Google;
use Stevenmaguire\OAuth2\Client\Provider\Microsoft;
use App\Traits\ApiResponseFormatTrait;
class EmailConfig extends Model
{
    use HasFactory;
    use ApiResponseFormatTrait;
     /*
     * Check Client ID Exist or Not: Function()
     * @Input Paramenter : clientId
     * @Return Parameter : bool
     */
    public static function isClientIdExists($mailId, $userId)
    {
        $emailData = ClientConfig::join('provider_configs', 'provider_configs.provider_guid', '=', 'client_configs.provider_id')
            ->select('client_configs.username', 'client_configs.password', 'client_configs.client_id', 'client_configs.client_secret', 'client_configs.refresh_token', 'provider_configs.auth_type', 'provider_configs.host_name', 'provider_configs.port', 'provider_configs.smtp_secure', 'provider_configs.smtp_auth', 'provider_configs.provider')
            ->where('client_configs.mail_id', $mailId)
            ->where('client_configs.user_id', $userId)
            ->first();
        return $emailData;
    }

    /*
     * Compose & Send E-mail: Function()
     * @Input Paramenter : AuthType, EmailData
     * @Return Parameter : API
     */
    public static function composeMail($emailData, $to_address, $subject, $body, $Cc, $Bcc, $attachments)
    {
        require base_path("vendor/autoload.php");
        $mail = new PHPMailer(true);
        try {
            // Email server settings
            $mail->isSMTP();
            $mail->CharSet = PHPMailer::CHARSET_UTF8;
            $mail->SMTPDebug = 0;
            $mail->Host = $emailData->host_name; //smtp host
            $mail->Port = $emailData->port; //port - 587/465
            $mail->SMTPSecure = $emailData->smtp_secure; //encryption - ssl/tls
            $mail->SMTPAuth = $emailData->smtp_auth;
            $mail->AuthType =  $emailData->auth_type;
            $mail->setFrom($emailData->username);
            $toAddress = explode(",", $to_address);

            $rules = [
                'toAddress' => 'required|email',
            ];
            if ($toAddress) {
                foreach ($toAddress as $toAddress) {
                    $data = [
                        'toAddress' => $toAddress
                    ];
                    $validator = \Validator::make($data, $rules);
                    if ($validator->fails()) {
                        return jsonResponse(false, 400, 'Invalid Input', '', $validator->errors());
                    }
                    $mail->addAddress(trim($toAddress));
                }
            }

            $mail->addReplyTo($emailData->username);
            $mail->isHTML(true);  // Set email content format to HTML
            $mail->Subject = $subject;
            //$mail->Body    = $request->body;
            //$body = "Hello <b>Everyone</b>,<br>$body<br><br><br>Thank you <br>MetaMax Team.";
            $mail->msgHTML($body);
            if (isset($Cc)) {
                $receipientsCc = explode(",", $Cc);
                // foreach ($receipientsCc as $receipientcc){
                // $mail->addCC(trim($receipientcc));
                $rules = [
                    'receipientsCc' => 'required|email',
                ];
                if ($Cc) {
                    foreach ($receipientsCc as $receipientsCc) {
                        $data = [
                            'receipientsCc' => $receipientsCc
                        ];
                        // dd($toAddress);
                        $validator = \Validator::make($data, $rules);
                        if ($validator->fails()) {
                            // dd("tests");
                            return ApiResponseFormatTrait::jsonResponse(false, 400, 'Invalid Input', '', $validator->errors());
                        }
                        $mail->addCC(trim($receipientsCc));
                    }
                }
                // }
            }
            if (isset($Bcc)) {
                $receipientsBcc = explode(",", $Bcc);
                //     foreach ($receipientsBcc as $receipientbcc){
                //     $mail->addBCC(trim($receipientbcc));
                // }
                $rules = [
                    'receipientsBcc' => 'required|email',
                ];
                if ($Bcc) {
                    foreach ($receipientsBcc as $receipientsBcc) {
                        $data = [
                            'receipientsBcc' => $receipientsBcc
                        ];
                        // dd($toAddress);
                        $validator = \Validator::make($data, $rules);
                        if ($validator->fails()) {
                            // dd("tests");
                            return ApiResponseFormatTrait::jsonResponse(false, 400, 'Invalid Input', '', $validator->errors());
                        }
                        $mail->addBCC(trim($receipientsBcc));
                    }
                }
            }

            // dd($attachments);
            if (count($attachments) > 0) {
                for ($i = 0; $i < count($attachments['tmp_name']); $i++) {
                    $mail->addAttachment($attachments['tmp_name'][$i], $attachments['name'][$i]);
                }
            }

            if (strtolower($emailData->auth_type) == "password") {
                $mail->Username = $emailData->username; //sender username
                $mail->Password =  $emailData->password;
            } else if (strtolower($emailData->auth_type) == "xoauth2") {
                if (strtolower($emailData->host_name) == "smtp.gmail.com") {
                    $provider = new Google(
                        [
                            'clientId' => $emailData->client_id,
                            'clientSecret' => $emailData->client_secret,
                        ]
                    );
                } else if (strtolower($emailData->host_name) == "smtp-mail.outlook.com" || strtolower($emailData->host_name) == "smtp.office365.com") {
                    $provider = new Microsoft(
                        [
                            'clientId' => $emailData->client_id,
                            'clientSecret' => $emailData->client_secret,
                        ]
                    );
                }
                $mail->setOAuth(
                    new OAuth(
                        [
                            'provider'          => $provider,
                            'clientId'          => $emailData->client_id,
                            'clientSecret'      => $emailData->client_secret,
                            'refreshToken'      => $emailData->refresh_token,
                            'userName'          => $emailData->username
                        ]
                    )
                );
            }

            if ($mail->send()) {
                return ApiResponseFormatTrait::jsonResponse(true, 200, 'Email Sent Successfully.', '', '');
            }
        } catch (Error $e) {
            return ApiResponseFormatTrait::jsonResponse(false, 400, 'Something went wrong.', '', $e);
        }
    }
}

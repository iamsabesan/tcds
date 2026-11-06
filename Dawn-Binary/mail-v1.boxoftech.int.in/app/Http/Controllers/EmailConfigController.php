<?php

namespace App\Http\Controllers;
use App\Traits\ApiResponseFormatTrait;
use Illuminate\Http\Request;
use App\Models\EmailConfig;
use Illuminate\Support\Facades\Validator;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Str;

class EmailConfigController extends Controller
{
    protected $emailConfig;
    public function __construct(EmailConfig $emailConfig)
    {
        $this->emailConfig = $emailConfig;
    }

    //To Send Mail Method((Gmail-App Password, Xauth2), (Outlook-App Password, Xauth2), Custom Domain)
    public function  sendEmail(Request $request){
        try {
            $validator = Validator::make($request->all(), [
                'mail_id'=> 'required',
                'to_address'     => 'required|email',
                'subject' => 'required',
                'body' => 'required',
            ]

            );
            if ($validator->fails()) {
                return $this->jsonResponse(false, 400, 'Required field missing', '', $validator->errors());
            }

            //Check Client ID Exist or Not
            $emailData = $this->emailConfig->isClientIdExists($request->mail_id,$request->userid);

            if(is_null($emailData) || empty($emailData)){
              return  $this->jsonResponse(false,400, 'Invalid Client ID.', '', '');
            }

            //if(!isset($request['files'])){
           // 	$request['files']= [];
            //}else{
            //	$request['files'] = $_FILES['files'];
            //}
			if(!isset($request['files'])){
            	$request['files']= [];
            }else{
            	$request['files'] = isset($_FILES['files'])?$_FILES['files'] :$request['files'] ;
            }
			
            return $this->emailConfig->composeMail($emailData, $request->to_address, $request->subject, $request->body,$request->Cc, $request->Bcc, $request['files']);
        }
        catch (Error  $e) {
            return $this->jsonResponse(false, 400, 'Something went wrong!', '', $e);
        }

    }
}

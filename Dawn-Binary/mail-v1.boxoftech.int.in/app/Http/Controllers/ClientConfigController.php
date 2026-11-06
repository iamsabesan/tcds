<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\ClientConfig;
use Illuminate\Support\Facades\Validator;
use Illuminate\Support\Facades\DB;
use JWTAuth;
use Illuminate\Support\Str;
class ClientConfigController extends Controller
{
    protected $clientConfig;
    public function __construct(ClientConfig $clientConfig)
    {
        $this->clientConfig = $clientConfig;
    }

     //To create client configuration
    public function create(Request $request)
    {
        try {
            $queryResult = '';
        $validator = Validator::make($request->all(), [
            'username'     => 'required',
            'provider_id' => 'required',
            ]);
            if ($validator->fails()) {
            return $this->jsonResponse(false, 400, 'Required field missing', '', $validator->errors());
        }

        /* Begin Provider Validate Section*/
        if($request->get('provider_id', 0)){
            $queryResult = DB::table('provider_configs')->select('auth_type')->where('provider_guid', '=', $request->provider_id)->first();

            if(is_null($queryResult) || empty($queryResult)){
                return $this->jsonResponse(false,400, 'Invalid Provider ID.', '', '');
            }

            elseif(strtolower($queryResult->auth_type) == "password"){
                $validator = Validator::make($request->all(), [
                    'password'     => 'required',
                    ]);
                    if ($validator->fails()) {
                        return $this->jsonResponse(false, 400, 'Required field missing', '', $validator->errors());
                    }
            }elseif(strtolower($queryResult->auth_type) == "xoauth2"){
                $validator = Validator::make($request->all(), [
                    'client_id'     => 'required',
                    'client_secret'  => 'required',
                    'refresh_token'   => 'required',
                    ]);
                    if ($validator->fails()) {
                    return $this->jsonResponse(false,400, 'Required field missing', '', $validator->errors());
                }
            }
        }
        /* End Provider Validate Section*/

        /* Begin Validate Section for existing value from db */
        $isDataExist = $this->clientConfig->isDataExists($queryResult->auth_type, $request);

            if(!is_null($isDataExist) || !empty($isDataExist)){
                return $this->jsonResponse(false,400, 'Client Config Already Exists.', '', '');
            }
        /* End Validate Section for existing value from db */

        $clientconfig = new ClientConfig();
        $clientconfig->mail_id = $this->clientConfig->check_mailid(Str::uuid());
        $clientconfig->username = $request->username;
        $clientconfig->password = (strtolower($queryResult->auth_type) == "password") ? ($request->password) : null;
        $clientconfig->client_id = (strtolower($queryResult->auth_type) == "xoauth2") ? ($request->client_id) : null;
        $clientconfig->client_secret = (strtolower($queryResult->auth_type) == "xoauth2") ? $request->client_secret : null;
        $clientconfig->refresh_token = (strtolower($queryResult->auth_type) == "xoauth2") ? $request->refresh_token : null;
        $clientconfig->provider_id = $request->provider_id;
        $clientconfig->user_id = $request->userid;
        $clientconfig->save();
        return $this->jsonResponse(true, 200, 'Client Configuration Added Successfully',['data' => $clientconfig->mail_id], '');
        }catch (\Exception $e) {
            return $this->jsonResponse(false, 400, 'Something went wrong!', '', $e);
        }
    }

public function view(Request $request) {
    try {
        return $this->jsonResponse(true, 200, 'Data Fetched Successfully', $this->clientConfig->getClientConfigData($request->get('client_id', 0)), "");
    }catch (\Exception $e) {
        return $this->jsonResponse(false, 400, 'Something went wrong!', '', $e);
        }
    }
 //To Update client configuration
 public function update(Request $request)
    {
        try {
            $queryResult = '';
            $validator = Validator::make($request->all(), [
                'username'     => 'required',
                'provider_id' => 'required',
                'mail_id' => 'required',
                ]);
                if ($validator->fails()) {
                return $this->jsonResponse(false, 400, 'Required field missing', '', $validator->errors());
            }

            /* Begin Provider Validate Section*/
            if($request->get('mail_id', 0)){
                $queryResult = DB::table('provider_configs')->select('auth_type')->where('provider_guid', '=', $request->provider_id)->first();

                if(is_null($queryResult) || empty($queryResult)){
                    return $this->jsonResponse(false,400, 'Invalid Provider ID.', '', '');
                }

                elseif(strtolower($queryResult->auth_type) == "password"){
                    $validator = Validator::make($request->all(), [
                        'password'     => 'required',
                        ]);
                        if ($validator->fails()) {
                            return $this->jsonResponse(false, 400, 'Required field missing', '', $validator->errors());
                        }
                }elseif(strtolower($queryResult->auth_type) == "xoauth2"){
                    $validator = Validator::make($request->all(), [
                        'client_id'     => 'required',
                        'client_secret'  => 'required',
                        'refresh_token'   => 'required',
                        ]);
                        if ($validator->fails()) {
                        return $this->jsonResponse(false,400, 'Required field missing', '', $validator->errors());
                    }
                }
            }
            /* End Provider Validate Section*/
             /* Begin Validate Section for existing value from db */
            $isDataExist = $this->clientConfig->isClientExists($request->mail_id);
            if(is_null($isDataExist) || empty($isDataExist)){
                return $this->jsonResponse(false,400, 'Client does not exists.', '', '');
            }
           /* End Validate Section for existing value from db */
           $affectedRows = ClientConfig::where('mail_id', '=', $request->mail_id)
           ->update(
               array(
                   'username' =>  $request->username,
                   'password' => (strtolower($queryResult->auth_type) == "password") ? ($request->password) : null,
                   'client_id' => (strtolower($queryResult->auth_type) == "xoauth2") ? ($request->client_id) : null,
                   'client_secret' => (strtolower($queryResult->auth_type) == "xoauth2") ? $request->client_secret : null,
                   'refresh_token' => (strtolower($queryResult->auth_type) == "xoauth2") ? $request->refresh_token : null,
                   'provider_id' =>  $request->provider_id
                ));

         if($affectedRows> 0){
         return  $this->jsonResponse(true, 200, 'Client Updated Successfully', '', '');
         }

        }catch (Error $e) {
            return $this->jsonResponse(false, 400, 'Something went wrong!', '', $e);
        }
    }
}

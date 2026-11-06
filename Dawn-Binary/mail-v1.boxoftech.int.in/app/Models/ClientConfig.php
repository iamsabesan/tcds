<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Facades\DB;
class ClientConfig extends Model
{
    use HasFactory;
    protected $fillable = ['mail_id','username','password','client_id','client_secret','refresh_token','provider_id'];

    /*
     * Check Client ID Exist or Not: Function()
     * @Input Paramenter : clientId
     * @Return Parameter : Email Data
     */
    public static function isClientIdExists($clientId){

        $emailData = ClientConfig::join('provider_configs', 'provider_configs.provider_guid', '=', 'client_configs.provider_id')
            ->select('client_configs.username','client_configs.password','client_configs.client_id','client_configs.client_secret','client_configs.refresh_token', 'provider_configs.auth_type','provider_configs.host_name','provider_configs.port','provider_configs.smtp_secure','provider_configs.smtp_auth','provider_configs.provider')
            ->where('client_configs.mail_id', $clientId)
            ->first();

        return $emailData;
    }

    /*
     * Check Data Exist or Not via Auth Type: Function()
     * @Input Paramenter : auth_type, username, password, provider_id
     * @Return Parameter : Client Config Data
     */
    public static function isDataExists($auth_type, $request){
        if(strtolower($auth_type) == "password"){
            $result = DB::table('client_configs')
                                ->where('username', '=', $request->username)
                                ->where('password', '=', $request->password)
                                ->where('provider_id', '=', $request->provider_id)
                                ->first();
            return $result;
        }elseif(strtolower($auth_type) == "xoauth2"){
            $result = DB::table('client_configs')
                                ->where('username', '=', $request->username)
                                ->where('client_id', '=', $request->client_id)
                                ->where('client_secret', '=', $request->client_secret)
                                ->where('refresh_token', '=', $request->refresh_token)
                                ->where('provider_id', '=', $request->provider_id)
                                ->first();
            return $result;
        }
    }

    /*
     * Get Client Config Data All (or) ClientId: Function()
     * @Input Paramenter : clientId
     * @Return Parameter : Client Config Data
     */
    public static function getClientConfigData($clientId){
        $clientConfigData = '';

        if($clientId){
        $clientConfigData = DB::table('client_configs as c')
        ->select('c.mail_id','c.username','c.provider_id','c.created_at', 'auth_type','host_name','port','smtp_secure','smtp_auth','provider','c.created_at')
        ->join('provider_configs', 'provider_configs.provider_guid', '=', 'c.provider_id')
        ->where('c.mail_id', $clientId)
        ->first();
       }else{
          $clientConfigData = DB::table('client_configs as c')
          ->select('c.mail_id','c.username','c.provider_id','c.created_at', 'auth_type','host_name','port','smtp_secure','smtp_auth','provider','c.created_at')
          ->join('provider_configs', 'provider_configs.provider_guid', '=', 'c.provider_id')
          ->get();
        }
        return $clientConfigData;
    }
     /*
     * Check Clientid Exist or Not: Function()
     * @Input Paramenter : Clientid
     * @Return Parameter : Client Data
     */
    public static function isClientExists($mailId){

        $result = DB::table('client_configs')
                            ->where('mail_id', '=', $mailId)
                            ->first();
        return $result;

    }
/*
     * Check Clientid Exist or Not: Function()
     * @Input Paramenter : Mailid
     * @Return Parameter : uuid
     */
    public function check_mailid($id)
    {
        $result = ClientConfig::where('mail_id', $id)->count();

        if($result){
            return $this->check_mailid(Str::uuid());
        }else{
            return $id;
        }
    }
}

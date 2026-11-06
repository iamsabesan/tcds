<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class ProviderConfig extends Model
{
    use HasFactory;
    protected $fillable = ['provider_guid','provider','host_name','port','smtp_secure','smtp_auth','auth_type'];

    /*
     * Check Data Exist or Not: Function()
     * @Input Paramenter : Request[]
     * @Return Parameter : Provider Data
     */
    public static function isDataExists($request){

        $result = DB::table('provider_configs')
                            ->where('host_name', '=', $request->host_name)
                            ->where('port', '=', $request->port)
                            ->where('smtp_secure', '=', $request->smtp_secure)
                            ->where('smtp_auth', '=', $request->smtp_auth)
                            ->where('auth_type', '=', $request->auth_type)
                            ->first();
        return $result;

    }
     /*
     * Check Providerid Exist or Not: Function()
     * @Input Paramenter : ProviderId
     * @Return Parameter : Provider Data
     */
    public static function isProviderIdExists($providerId){

        $result = DB::table('provider_configs')
                            ->where('provider_guid', '=', $providerId)
                            ->first();
        return $result;

    }
       /*
     * Check AuthType Valid or Not: Function()
     * @Input Paramenter : AuthType
     * @Return Parameter : True Or False
     */
    public static function isAuthTypeExist($AuthType){
        if(strtolower($AuthType) == "password" || strtolower($AuthType) == "xoauth2"){
           return true;
        }else{
            return false;
        }
    }
}

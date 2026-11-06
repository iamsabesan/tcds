<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\ProviderConfig;
use Illuminate\Support\Facades\Validator;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Str;

class ProviderConfigController extends Controller
{
    protected $providerConfig;
    public function __construct(ProviderConfig $providerConfig)
    {
        $this->providerConfig = $providerConfig;
    }

    //To create provider configuration
    public function create(Request $request) {
    try {
    $validator = Validator::make($request->all(), [
        'provider'     => 'required',
        'host_name'     => 'required',
        'port' => 'required',
        'smtp_secure' => 'required',
        'smtp_auth' => 'required',
        'auth_type' => 'required',
        ]);
        if ($validator->fails()) {
        return $this->jsonResponse(400, 'Required field missing', '', $validator->errors());
        }

        /* Begin Validate Section for existing value from db */
        $isDataExist = $this->providerConfig->isDataExists($request);
            if(!is_null($isDataExist) || !empty($isDataExist)){
                return $this->jsonResponse(false,400, 'Provider Already Exists.', '', '');
            }
        /* End Validate Section for existing value from db */

        /* Begin Auth Type Validation value from request  */
        $isAuthTypeExist = $this->providerConfig->isAuthTypeExist($request->auth_type);
        if($isAuthTypeExist != "true"){
            return $this->jsonResponse(false,400, 'InValid Input.', '', '');
        }
        /* End Auth Type Validation value from request */
        $provideconfig = new ProviderConfig();
        // $uuid = Str::uuid();
        $provideconfig->provider_guid = Str::uuid();
        $provideconfig->provider = $request->provider;
        $provideconfig->host_name = $request->host_name;
        $provideconfig->port = $request->port;
        $provideconfig->smtp_secure = $request->smtp_secure;
        $provideconfig->smtp_auth = $request->smtp_auth;
        $provideconfig->auth_type = $request->auth_type;
        $provideconfig->save();
        return  $this->jsonResponse(true, 200, 'Provider Added Successfully', '', '');
    }catch (\Exception $e) {
            return  $this->jsonResponse(false, 400, '', '', $e);
        }
    }

    //To view provider details list
    public function view(Request $request) {
        try {
            $provideconfigresult = '';
            if($request->get('provider_id', 0)){
                $provideconfigresult = DB::table('provider_configs')->where('provider_guid', '=', $request->provider_id)->first();
            }else{
                $provideconfigresult = ProviderConfig::all();
            }
            return $this->jsonResponse(true, 200, 'Data Fetched Successfully', $provideconfigresult, "");

        }catch (\Exception $e) {
            return  $this->jsonResponse(false, 400, '', '', $e);
            }
    }

    //To update  provider configuration
    public function update(Request $request) {
        try{
        $validator = Validator::make($request->all(), [
            'provider_guid'=>'required',
            'provider'     => 'required',
            'host_name'     => 'required',
            'port' => 'required',
            'smtp_secure' => 'required',
            'smtp_auth' => 'required',
            'auth_type' => 'required',
            ]);
            if ($validator->fails()) {
            return $this->jsonResponse(false,400, 'Required field missing', '', $validator->errors());
            }

            /* Begin Validate Section for existing value from db */
            $isDataExist = $this->providerConfig->isProviderIdExists($request->provider_guid);
                if(is_null($isDataExist) || empty($isDataExist)){
                    return $this->jsonResponse(false,400, 'Provider does not exists.', '', '');
                }
            /* End Validate Section for existing value from db */
            $affectedRows = ProviderConfig::where('provider_guid', '=', $request->provider_guid)
                                            ->update(
                                                array(
                                                    'provider' =>  $request->provider,
                                                    'host_name' =>  $request->host_name,
                                                    'port' =>  $request->port,
                                                    'smtp_secure' =>  $request->smtp_secure,
                                                    'smtp_auth' =>  $request->smtp_auth,
                                                    'auth_type' =>  $request->auth_type
                                                 ));

            if($affectedRows> 0){
                return  $this->jsonResponse(true, 200, 'Provider Updated Successfully', '', '');
                }
            }catch (Error $e) {
            return  $this->jsonResponse(false, 400, '', '', $e);
            }
    }
}

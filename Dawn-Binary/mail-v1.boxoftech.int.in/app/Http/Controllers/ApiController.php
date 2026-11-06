<?php

namespace App\Http\Controllers;
use JWTAuth;
use App\Models\User;
use Illuminate\Http\Request;
use Tymon\JWTAuth\Exceptions\JWTException;
use Symfony\Component\HttpFoundation\Response;
use Illuminate\Support\Facades\Validator;
use Config;
use Illuminate\Support\Str;
use DB;

class ApiController extends Controller
{
    public function register(Request $request)
    {
        try {
    	//Validate data
        $data = $request->only('name', 'email', 'password');
        $validator = Validator::make($data, [
            'name' => 'required|string',
            'email' => 'required|email|unique:users',
            'password' => 'required|string|min:6|max:50'
        ]);

        //Send failed response if request is not valid
        if ($validator->fails()) {
            return response()->json(['error' => $validator->messages()], 200);
        }

        //Request is valid, create new user
        $user = User::create([
        	'name' => $request->name,
        	'email' => $request->email,
            'userid'=> Str::uuid(),
        	'password' => bcrypt($request->password)
        ]);
        return $this->jsonResponse(true, 200, 'User created successfully',['user_id' => $user->userid], '');

    }catch (\Exception $e) {
        return $this->jsonResponse(false, 400, 'Something went wrong!', '', $e);
    }
        //User created, return success response

    }

     public function authenticate(Request $request)
    {
        $credentials = $request->only('user_name', 'password');

        // Determine whether the provided input is an email or a username
        $loginField = filter_var($credentials['user_name'], FILTER_VALIDATE_EMAIL) ? 'email' : 'userid';

        if($loginField == 'email'){
            $validator = Validator::make($credentials, [
                'user_name' => 'required|email',
                'password' => 'required|string|min:6|max:50'
            ]);
        }else{
            $validator = Validator::make($credentials, [
                'user_name' => 'required|max:36',
                'password' => 'required|string|min:6|max:50'
            ]);
        }

        if ($validator->fails()) {
            return response()->json(['error' => $validator->messages()], 200);
        }

        $credentials[$loginField] = $credentials['user_name'];

        // Remove the identifier from the credentials array
        unset($credentials['user_name']);

        try {
            JWTAuth::factory()->setTTL(15); //minutes
            if (! $token = JWTAuth::attempt($credentials)) {
             return $this->jsonResponse(false, 401, 'Invalid credentials', '', '');
            }

        } catch (JWTException $e) {
            return $this->jsonResponse(false, 500, 'Could not create token', '', '');
        }

 		//Token created, return with success response and jwt token
         $userData = [
            'access_token' => $token,
            'token_type' => 'bearer',
            'expires_in' => auth('api')->factory()->getTTL(),
            'user' => auth()->user()
         ];
         return  $this->jsonResponse(true, 200, '',$userData, '');
   }

    public function logout(Request $request)
    {
        //valid credential
        $validator = Validator::make($request->only('token'), [
            'token' => 'required'
        ]);

        //Send failed response if request is not valid
        if ($validator->fails()) {
         return $this->jsonResponse(false,400, 'Required field missing', '', $validator->errors());
        }

		//Request is validated, do logout
        try {
            JWTAuth::invalidate($request->token);

            return response()->json([
                'success' => true,
                'message' => 'User has been logged out'
            ]);
        } catch (JWTException $exception) {
            return $this->jsonResponse(false, 400, 'Sorry, user cannot be logged out', '', '');
          }
    }

    public function get_user(Request $request)
    {
        $this->validate($request, [
            'token' => 'required'
        ]);
        $users = User::all()->where('role','!=','admin')->each(function ($user) {
            $user->makeHidden('password');
        });
      return  $this->jsonResponse(true, 200, '',$users, '');
     }
}

<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\ClientConfigController;
use App\Http\Controllers\ProviderConfigController;
use App\Http\Controllers\EmailConfigController;
use App\Http\Controllers\ApiController;
/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/


Route::post('login', [ApiController::class, 'authenticate']);
Route::post('register', [ApiController::class, 'register']);


Route::group(['middleware' => ['jwt.verify']], function() {
    Route::get('getclientlist', [ApiController::class, 'get_user']);
    Route::post('provider/create', [ProviderConfigController::class, 'create'])->name('create.action');
    Route::put('provider/update', [ProviderConfigController::class, 'update'])->name('update.action');
    Route::get('provider/list', [ProviderConfigController::class, 'view'])->name('view.action');
    Route::post('client/create', [ClientConfigController::class, 'create'])->name('create.action');
    Route::post('client/update', [ClientConfigController::class, 'update'])->name('update.action');
    Route::get('client/list', [ClientConfigController::class, 'view'])->name('view.action');
    });

    Route::middleware([ClientMiddleware::class])->group(function () {
     Route::post('email/send', [EmailConfigController::class, 'sendEmail'])->name('composeemail.action');
    });
// Route::middleware('auth:sanctum')->get('/user', function (Request $request) {
//     return $request->user();
// });

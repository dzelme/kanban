import * as React from 'react';
import 'isomorphic-fetch';
import { Credentials, Board, Value, StatisticsEntry, JiraConnectionLogEntry, BoardPresentation, CardColor } from './Interfaces';

function handleResponse(response: Response): Promise<any> {
    if (response.ok) return response.json();

    if (response.status == 401) {
        window.open('./login', '_self');
    }

    console.error(response);
    throw Error(response.statusText);
}

export class ApiClient {

    static tokenName = 'JwtToken';

    // helper post method
    static post(link, data): Promise<any> {
        return ApiClient.unhandledPost(link, data)
            .then(handleResponse);
    }

    // helper post method
    static unhandledPost(link, data): Promise<any> {
        return fetch(link, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'authorization': 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName),
            },
            body: JSON.stringify(data)
        })
    }

    // helper get method
    static get(link): Promise<any> {
        return fetch(link, {
            headers: {
                'Accept': 'application/json',
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        })
        .then(handleResponse);
    }

    // helper delete method
    static delete(link) {
        return fetch(link, {
            method: 'DELETE',
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        })
    }

    // AccountController: Checks the credentials submitted by user.
    static login(credentials: Credentials): Promise<boolean> {
        return ApiClient.unhandledPost('api/account/login', credentials)
            .then(response => response.json())
            .then(json => {
                sessionStorage.setItem(ApiClient.tokenName, json.token);
                return true;
            })
            .catch(err => {
                return false;
            });
    }

    // AccountController
    static checkCredentials(credentials: Credentials): Promise<Response> {
        return ApiClient.unhandledPost('api/account/CheckCredentials', credentials)
    }

    // AccountController: Checks if the current user has a valid JWT token
    static hasValidJwt(redirect = true ): Promise<boolean> {
        var p = fetch('api/account/hasValidJwt', {
            headers: {
                'Accept': 'application/json',
                authorization: 'Bearer ' + sessionStorage.getItem(ApiClient.tokenName)
            }
        });
        if (redirect) {
            return p.then(handleResponse).then(() => true);
        }

        return p.then(response => response.ok);
    }

    // SampleDataController: Gets board list
    static boardList(credentials: Credentials): Promise<Value[]> {
        return ApiClient.post('api/SampleData/BoardList', credentials) as Promise<Value[]>;
    }

    // SampleDataController: Gets color list
    static colorList(id: number, credentials: Credentials): Promise<CardColor[]> {
        return ApiClient.post('api/SampleData/ColorList?id=' + id.toString(), credentials) as Promise<CardColor[]>;
    }

    // SampleDataController: Gets board data
    static boardData(id: number, credentials: Credentials): Promise<Board> {
        return ApiClient.post('api/SampleData/BoardData?id=' + id.toString(), credentials) as Promise<Board>;
    }

    // PresentationsController
    static getPresentations(): Promise<BoardPresentation[]> {
        return ApiClient.get('api/admin/presentations') as Promise<BoardPresentation[]>;
    }

    // PresentationsController
    static getPresentation(id: string): Promise<BoardPresentation> {  //400?
        return ApiClient.get('api/admin/presentations/' + id) as Promise<BoardPresentation>;
    }

    // PresentationsController
    static savePresentation(boardPresentation: BoardPresentation): Promise<BoardPresentation> {
        return ApiClient.post('api/admin/presentations/', boardPresentation) as Promise<BoardPresentation>;
    }

    // PresentationsController
    static deletePresentation(id: string) {
        return ApiClient.delete('api/admin/presentations/' + id);
    }

    // VersionController
    static getVersion(): Promise<string> {
        return ApiClient.get('api/version') as Promise<string>;
    }

    // StatisticsController
    static statisticsList(): Promise<StatisticsEntry[]> {
        return ApiClient.get('api/Statistics/GetStatisticsList')
    }

    // StatisticsController
    static networkStatistics(id: string): Promise<JiraConnectionLogEntry[]> {
        return ApiClient.post('api/Statistics/GetNetworkStatisticsList', id)
    }

    // StatisticsController
    static saveToStatistics(id: string, name: string) {
        return ApiClient.unhandledPost('api/Statistics/SaveToStatistics?id=' + id, name)
    }

    // SettingsController
    static saveUserSettings(boardList: Value[], id: string) {
        return ApiClient.post('api/Settings/SaveUserSettings?id=' + id, boardList)
    }
}
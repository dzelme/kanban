import * as React from 'react';
import 'isomorphic-fetch';
import { Credentials, Board, Value, StatisticsDbModel, StatisticsPresentationModel, StatisticsBoardModel, StatisticsConnectionModel, BoardPresentation, CardColor } from './Interfaces';

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

    // SampleDataController: Gets board list using credentials
    static boardListFromCredentials(credentials: Credentials): Promise<Value[]> {
        return ApiClient.post('api/SampleData/BoardListFromCredentials', credentials) as Promise<Value[]>;
    }

    // SampleDataController: Gets board list using presentation id
    static boardListFromId(id: string): Promise<Value[]> {
        return ApiClient.post('api/SampleData/BoardListFromId', id) as Promise<Value[]>;
    }

    // SampleDataController: Gets board data
    static boardData(boardId: string, presentationId: string): Promise<Board> {
        return ApiClient.post('api/SampleData/BoardData?boardId=' + boardId, presentationId) as Promise<Board>;
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
    static savePresentation(boardPresentation: BoardPresentation): Promise<any> {
        return ApiClient.unhandledPost('api/admin/presentations/', boardPresentation) as Promise<any>;
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
    static saveViewStatistics(stats: StatisticsDbModel) {
        return ApiClient.post('api/Statistics/SaveViewStatistics', stats)
    }

    // StatisticsController
    static statisticsPresentationList(): Promise<StatisticsPresentationModel[]> {
        return ApiClient.get('api/Statistics/GetStatisticsPresentationList')
    }

    // StatisticsController
    static statisticsBoardList(presentationId: string): Promise<StatisticsBoardModel[]> {
        return ApiClient.get('api/Statistics/GetStatisticsBoardList?presentationId=' + presentationId)
    }

    // StatisticsController
    static statisticsConnectionList(presentationId: string, boardId: string): Promise<StatisticsConnectionModel[]> {
        return ApiClient.post('api/Statistics/GetStatisticsConnectionList?boardId=' + boardId, presentationId)
    }
}
import * as React from 'react';
import 'isomorphic-fetch';
import jwt_decode from 'jwt-decode';
import { RouteComponentProps } from 'react-router';
import { CreatePresentationState, Value, BoardPresentation } from './Interfaces';
import { ApiClient } from './ApiClient';

export class CreatePresentation extends React.Component<RouteComponentProps<{}>, CreatePresentationState> {
    constructor() {
        super();

        this.state = {
            boardPresentation: {
                id: "",
                title: "",
                owner: "",
                credentials: {
                    username: "",
                    password: "",
                },
                boards: {
                    values: []
                }
            },
            loading: true,
            authenticated: true
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleAuth = this.handleAuth.bind(this);
        this.handleFetch = this.handleFetch.bind(this);
        this.postPresentation = this.postPresentation.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleChangeBoardVisibility = this.handleChangeBoardVisibility.bind(this);
        this.handleChangeBoardTimes = this.handleChangeBoardTimes.bind(this);
    }

    handleAuth(event) {
        event.preventDefault();

        var newBoardPresentation = {
            id: this.state.boardPresentation.id,
            title: this.state.boardPresentation.title,
            owner: jwt_decode(sessionStorage.getItem('JwtToken'))['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
            credentials: this.state.boardPresentation.credentials,
            boards: {
                values: []  // removed not to trigger invalid ModelState
            }
        }

        ApiClient.savePresentation(newBoardPresentation)
            .then(response => {
                if (response.status == 200 || response.status == 400) {
                    response.json().then(json => {
                        newBoardPresentation.id = json.id;
                        newBoardPresentation.boards = this.state.boardPresentation.boards;
                        this.setState({ boardPresentation: newBoardPresentation, authenticated: true }, this.handleFetch);
                    })
                }
                else {
                    this.setState({ authenticated: false });
                }
            });
    }

    handleFetch() {
        ApiClient.boardListFromCredentials(this.state.boardPresentation.credentials)
            .then(data => {
                this.setState({
                    boardPresentation: {
                        id: this.state.boardPresentation.id,
                        title: this.state.boardPresentation.title,
                        owner: this.state.boardPresentation.owner,
                        credentials: this.state.boardPresentation.credentials,
                        boards: {
                            values: data
                        }
                    },
                    loading: false
                });
            });
    }

    handleSubmit(event) {
        event.preventDefault();

        var val = new Array();

        this.state.boardPresentation.boards.values.map(board => {
            if (board.visibility == true) { val.push(board); }
        })

        let presentation = {
                id: this.state.boardPresentation.id,
                title: this.state.boardPresentation.title,
                owner: jwt_decode(sessionStorage.getItem('JwtToken'))['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
                credentials: this.state.boardPresentation.credentials,
                boards: {
                    values: val,
                }
            }
        this.postPresentation(presentation)
    }; 

    postPresentation(presentation: BoardPresentation) {
        ApiClient.savePresentation(presentation)
            .then(response => {
                if (response.status == 200) {
                    open('./admin/presentations', '_self')
                }
            });
    }

    handleChange(event) {
        const target = event.target;
        const name = target.name;

        if (name == 'title') {
            this.setState({ boardPresentation: { id: this.state.boardPresentation.id, title: event.target.value, owner: this.state.boardPresentation.owner, credentials: this.state.boardPresentation.credentials, boards: this.state.boardPresentation.boards } });
        }
        else if (name == 'username') {
            this.setState({ boardPresentation: { id: this.state.boardPresentation.id, title: this.state.boardPresentation.title, owner: this.state.boardPresentation.owner, credentials: { username: event.target.value, password: this.state.boardPresentation.credentials.password }, boards: this.state.boardPresentation.boards } });
        }
        else {
            this.setState({ boardPresentation: { id: this.state.boardPresentation.id, title: this.state.boardPresentation.title, owner: this.state.boardPresentation.owner, credentials: { username: this.state.boardPresentation.credentials.username, password: event.target.value }, boards: this.state.boardPresentation.boards } });
        }
    }

    handleChangeSettings(id: string, setting: string, value: number) {
        var newBoardList = this.state.boardPresentation.boards.values.concat();  // concat to clone (not reference the same)

        this.state.boardPresentation.boards.values.map((board, index) => {
            if (board.id.toString() == id) {
                if (setting == "visibility") {
                    newBoardList[index].visibility = !newBoardList[index].visibility;
                }
                if (setting == "timeShown") {
                    newBoardList[index].timeShown = value;
                }
                if (setting == "refreshRate") {
                    newBoardList[index].refreshRate = value;
                }

                this.setState({
                    boardPresentation: {
                        id: this.state.boardPresentation.id,
                        title: this.state.boardPresentation.title,
                        owner: this.state.boardPresentation.owner,
                        credentials: this.state.boardPresentation.credentials,
                        boards: {
                            values: newBoardList
                        }
                    },
                });
            }
        })
    }

    handleChangeBoardVisibility(id: string) {
        this.handleChangeSettings(id, "visibility", 0);
    }

    handleChangeBoardTimes(id: string, name: string, e) {
        var value = parseInt(e.target.value);

        if (name == 'timeShown') {
            this.handleChangeSettings(id, "timeShown", value);
        }
        if (name == 'refreshRate') {
            this.handleChangeSettings(id, "refreshRate", value);
        }
    }

    public render() {
        if (sessionStorage.getItem('JwtToken') === null) {
            return null;
        }

        let contents = this.state.loading
            ? null
            : (this.state.authenticated) ? CreatePresentation.renderBoardList(this.state.boardPresentation.boards.values, this.handleSubmit, this.handleChangeBoardVisibility, this.handleChangeBoardTimes) : null;

        let error = this.state.authenticated
            ? <h4>Brīdinājums! Lietotājvārds un parole tiks glabāti atklātā tekstā uz servera!</h4>
            : <h4>Nekorekts lietotājvārds un/vai parole!</h4>

        return <div className="top-padding">
            <h1>Izveidot prezentāciju</h1>

            <form name="presentation" onSubmit={this.handleAuth}>
                <div key="title" className="FormElement">
                    Nosaukums: <input id="title" required name="title" type="text" value={this.state.boardPresentation.title} onChange={this.handleChange} />
                </div>
                <div key="username" className="FormElement">
                    Lietotājvārds: <input id="username" required name="username" type="text" value={this.state.boardPresentation.credentials.username} onChange={this.handleChange} />
                </div>
                <div key="password" className="FormElement">
                    Parole: <input id="password" required name="password" type="password" value={this.state.boardPresentation.credentials.password} onChange={this.handleChange} />
                </div>
                <div className="FormButton"><button type="submit" className="btn btn-default">Apstiprināt</button></div>
            </form>
            {error}
            {contents}
        </div>;
    }

    private static renderBoardList(boardList: Value[], handleSubmit, handleChangeBoardVisibility, handleChangeBoardTimes) {
        return <div>
            <form name='boardlist' onSubmit={handleSubmit}>
                <table className='table'>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nosaukums</th>
                            <th className="CheckBox">Iekļaut prezentācijā</th>
                            <th>Attēlošanas laiks(s)</th>
                            <th>Atjaunošanas laiks(s)</th>
                        </tr>
                    </thead>
                    <tbody>
                        {boardList.map(board =>
                            <tr key={board.id + "row"}>
                                <td key={board.id + ""}>{board.id}</td>
                                <td key={board.id + "name"}>{board.name}</td>
                                <td key={board.id + "visibility"} className="CheckBox"><input name={board.id + "visibility"} type="checkbox" defaultChecked={board.visibility} onClick={() => handleChangeBoardVisibility(board.id)} /></td>
                                <td key={board.id + "timeShown"}><input name={board.id + "timeShown"} type="number" min="0" value={board.timeShown.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'timeShown', e)}/></td>
                                <td key={board.id + "refreshRate"}><input name={board.id + "refreshRate"} type="number" min="0" value={board.refreshRate.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'refreshRate', e)}/></td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <button className="btn btn-default" type="submit">Pievienot prezentāciju</button>
            </form>
        </div>
    }
}
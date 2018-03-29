﻿import * as React from 'react';
import 'isomorphic-fetch';
import jwt_decode from 'jwt-decode';
import { RouteComponentProps } from 'react-router';
import { CreatePresentationState, Value } from './Interfaces';
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
            boardlist: [],
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
        ApiClient.login(this.state.boardPresentation.credentials)
            .then(response => {
                if (response) {
                    this.setState({ authenticated: true }, this.handleFetch);
                }
                else {
                    this.setState({ authenticated: false });
                }
            });
    }

    handleFetch() {
        ApiClient.boardList(this.state.boardPresentation.credentials)
            .then(data => {
                this.setState({ boardlist: data, loading: false });
            });
    }

    handleSubmit(event) {
        event.preventDefault();

        var val = new Array();

        this.state.boardlist.map(board => {
            if (board.visibility == true) { val.push(board); }
        })

        this.setState({
            boardPresentation: {
                id: this.state.boardPresentation.id,
                title: this.state.boardPresentation.title,
                owner: jwt_decode(sessionStorage.getItem('JwtToken'))['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
                credentials: this.state.boardPresentation.credentials,
                boards: {
                    values: val,
                }
            }
        }, this.postPresentation)
    }

    postPresentation() {

        ApiClient.savePresentation(this.state.boardPresentation)
            .then(() => open('./admin/presentations', '_self'));
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

    handleChangeBoardVisibility(id: string) {
        var newBoardlist = this.state.boardlist;

        this.state.boardlist.map((board, index) => {
            if (board.id.toString() == id) {
                newBoardlist[index].visibility = !newBoardlist[index].visibility

                this.setState({
                    boardlist: newBoardlist
                });
            }
        })
    }

    handleChangeBoardTimes(id: string, name: string, e) {
        var newBoardlist = this.state.boardlist;
        var value = parseInt(e.target.value);

        if (name == 'timeShown') {

            this.state.boardlist.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardlist[index].timeShown = value*1000

                    this.setState({
                        boardlist: newBoardlist
                    });
                }
            })
        }
        else {
            this.state.boardlist.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardlist[index].refreshRate = value*1000

                    this.setState({
                        boardlist: newBoardlist
                    });
                }
            })
        }
    }

    public render() {
        if (sessionStorage.getItem('JwtToken') === null) {
            return null;
        }

        let contents = this.state.loading
            ? null
            : CreatePresentation.renderBoardList(this.state.boardlist, this.handleSubmit, this.handleChangeBoardVisibility, this.handleChangeBoardTimes);

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
                            <th>Tips</th>
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
                                <td key={board.id + "type"}>{board.type}</td>
                                <td key={board.id + "visibility"} className="CheckBox" ><input name={board.id + "visibility"} type="checkbox" defaultChecked={board.visibility} onClick={() => handleChangeBoardVisibility(board.id)} /></td>
                                <td key={board.id + "timeShown"}><input name={board.id + "timeShown"} type="number" value={(board.timeShown / 1000).toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'timeShown', e)} /></td>
                                <td key={board.id + "refreshRate"}><input name={board.id + "refreshRate"} type="number" value={(board.refreshRate / 1000).toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'refreshRate', e)}/></td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <button className="btn btn-default" type="submit">Pievienot prezentāciju</button>
            </form>
        </div>
    }
}
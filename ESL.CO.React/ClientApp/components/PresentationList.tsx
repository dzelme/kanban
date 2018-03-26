﻿import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { BoardPresentation } from './Interfaces';
import { Link } from 'react-router-dom';
import { ApiClient } from './ApiClient';

interface PresentationState {
    presentationList: BoardPresentation[];
    loading: boolean;
}

export class PresentationList extends React.Component<RouteComponentProps<{}>, PresentationState> {
    constructor() {
        super();

        this.state = { presentationList: [], loading: true };

        this.handleNew = this.handleNew.bind(this);

        ApiClient.getPresentations()
            .then(data => {
                this.setState({ presentationList: data, loading: false });
            });
    }

    handleNew() {
        open('./admin/createPresentation','_self');
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : PresentationList.renderPresentationList(this.state.presentationList, this.handleNew);

        return <div>
            {contents}
        </div>;
    }

    private static renderPresentationList(presentationList: BoardPresentation[], handleNew) {
        return <div className="top-padding">
            <h1>Prezentāciju saraksts</h1>
            <button onClick={handleNew} className="btn btn-default">Izveidot jaunu</button>
            <table className='table'>
                <thead style={styleHeader}>
                    <tr>
                        <th>ID</th>
                        <th>Nosaukums</th>
                        <th>Izveidotājs</th>
                        <th>Paneļi</th>
                    </tr>
                </thead>
                <tbody style={styleContent}>
                    {presentationList.map(presentation =>
                        <tr key={presentation.id + "row"}>
                            <td key={presentation.id + ""}><Link className="Link" to={"/p/" + presentation.id}>{presentation.id}</Link></td>
                            <td key={presentation.id + "title"}>{presentation.title}</td>
                            <td key={presentation.id + "owner"}>{presentation.owner}</td>
                            <td key={presentation.id + "boards"}>
                                {presentation.boards.values.map(board =>
                                    board.id + "; " 
                                )}
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    }
}

const styleHeader = {
    fontSize: '20px'
}

const styleContent = {
    fontSize: '15px'
}
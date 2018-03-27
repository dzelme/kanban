import * as React from 'react';
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
        this.handleDelete = this.handleDelete.bind(this);

        ApiClient.getPresentations()
            .then(data => {
                this.setState({ presentationList: data, loading: false });
            });
    }

    handleNew()
    {
        open('./admin/createPresentation','_self');
    }

    handleDelete(id: string) {
        ApiClient.deletePresentation(id)
        open('./admin/presentations', '_self');         
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : PresentationList.renderPresentationList(this.state.presentationList, this.handleNew, this.handleDelete);

        return <div>
            {contents}
        </div>;
    }

    private static renderPresentationList(presentationList: BoardPresentation[], handleNew, handleDelete) {
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
                        <th colSpan={2}>Darbības</th>
                    </tr>
                </thead>
                <tbody style={styleContent}>
                    {presentationList.map(presentation =>
                        <tr key={presentation.id + "row"}>
                            <td key={presentation.id + ""}><Link className="Link" to={"/p/" + presentation.id}>{presentation.id}</Link></td>
                            <td key={presentation.id + "title"}><Link className="Link" to={"/p/" + presentation.id}>{presentation.title}</Link></td>
                            <td key={presentation.id + "owner"}>{presentation.owner}</td>
                            <td key={presentation.id + "boards"}>
                                {presentation.boards.values.map((board, index) =>
                                    <Link key={index} className="Link" to={"/k/" + board.id}>{board.name}; </Link>
                                                  
                                )}
                            </td>
                            <td style={styleColumn}><Link to={'/admin/editPresentation/' + presentation.id}><button className="btn btn-default">Rediģēt</button></Link></td>
                            <td style={styleColumn}><button className="btn btn-default" onClick={() => handleDelete(presentation.id)}>Dzēst</button></td>
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

const styleColumn = {
    width: '30px'
}
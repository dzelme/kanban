import * as React from 'react';
import 'isomorphic-fetch';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import { PresentationListState, BoardPresentation } from './Interfaces';
import { ApiClient } from './ApiClient';

export class PresentationList extends React.Component<RouteComponentProps<{}>, PresentationListState> {
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
        open('./admin/presentations/create','_self');
    }

    handleDelete(id: string) {
        ApiClient.deletePresentation(id)
            .then(() => open('./admin/presentations', '_self'));
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
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nosaukums</th>
                        <th>Izveidotājs</th>
                        <th>Paneļi</th>
                        <th colSpan={2}>Darbības</th>
                    </tr>
                </thead>
                <tbody>
                    {presentationList.map(presentation =>
                        <tr key={presentation.id + "row"}>
                            <td key={presentation.id + ""}><Link className="LinkText" to={"/p/" + presentation.id}>{presentation.id}</Link></td>
                            <td key={presentation.id + "title"}><Link className="LinkText" to={"/p/" + presentation.id}>{presentation.title}</Link></td>
                            <td key={presentation.id + "owner"}>{presentation.owner}</td>
                            <td key={presentation.id + "boards"}>
                                {presentation.boards.values.map((board, index) =>
                                    <Link key={index} className="LinkText" to={"/k/" + presentation.id + "/" + board.id}>{board.name}; </Link>
                                                  
                                )}
                            </td>
                            <td className="EditDeletePresButton"><Link to={'/admin/presentations/edit/' + presentation.id}><button className="btn btn-default">Rediģēt</button></Link></td>
                            <td className="EditDeletePresButton"><button className="btn btn-default" onClick={() => handleDelete(presentation.id)}>Dzēst</button></td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    }
}
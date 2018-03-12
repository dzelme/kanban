import * as React from 'react';
import ColumnTitle from './ColumnTitle';
import ColumnFill from './ColumnFill';
import { Board } from './Interfaces';

export default class BoardTable extends React.Component<{ board: Board, boardTime: number }> {

    public render() {

        let columnCount = this.props.board.columns.length;

        return <section className="column">
            <div className="column-wrapper">{
                this.props.board.columns.map((column, index) =>
                    <h2 key={index}><ColumnTitle name={column.name} /></h2>
                )
            }
                {

                    this.props.board.columns.map((column, index) =>

                        <ColumnFill column={column} board={this.props.board} time={this.props.boardTime} index={index} columnCount={columnCount} />
                    )
                }
             </div>
        </section>
    }
}

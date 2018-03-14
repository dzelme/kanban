import * as React from 'react';
import ColumnTitle from './ColumnTitle';
import ColumnFill from './ColumnFill';
import { Board } from './Interfaces';

export default class BoardTable extends React.Component<{ board: Board, boardTime: number }> {

    public render() {

        let columnCount = this.props.board.columns.length;

        return <div>{
                this.props.board.columns.map((column, index) =>
                    <section className="column">
                        <div className="column-wrapper">
                            <h2 key={index}>{column.name}</h2>
                            <ColumnFill column={column} board={this.props.board} time={this.props.boardTime} index={index} columnCount={columnCount} />
                        </div>
                    </section>
                )
        }
        </div>
    }
}
